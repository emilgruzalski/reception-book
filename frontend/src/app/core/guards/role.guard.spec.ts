import { TestBed } from '@angular/core/testing';
import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';

import { AuthService } from '../services/auth.service';
import { roleGuard } from './role.guard';

describe('roleGuard', () => {
  const executeGuard: CanActivateFn = (...params) => TestBed.runInInjectionContext(() => roleGuard(...params));
  const route = { data: { roles: ['Administrator'] } } as unknown as ActivatedRouteSnapshot;
  const state = { url: '/users' } as RouterStateSnapshot;

  let authService: jasmine.SpyObj<AuthService>;

  beforeEach(() => {
    authService = jasmine.createSpyObj<AuthService>('AuthService', ['hasAnyRole']);
    TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      providers: [{ provide: AuthService, useValue: authService }]
    });
  });

  it('should allow users with a required role', () => {
    authService.hasAnyRole.and.returnValue(true);

    expect(executeGuard(route, state)).toBeTrue();
    expect(authService.hasAnyRole).toHaveBeenCalledWith(['Administrator']);
  });

  it('should redirect other users to the dashboard', () => {
    authService.hasAnyRole.and.returnValue(false);

    const result = executeGuard(route, state) as UrlTree;

    expect(TestBed.inject(Router).serializeUrl(result)).toBe('/dashboard');
  });
});
