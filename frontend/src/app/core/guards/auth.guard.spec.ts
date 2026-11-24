import { TestBed } from '@angular/core/testing';
import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';

import { AuthService } from '../services/auth.service';
import { authGuard } from './auth.guard';

describe('authGuard', () => {
  const executeGuard: CanActivateFn = (...params) => TestBed.runInInjectionContext(() => authGuard(...params));
  const route = {} as ActivatedRouteSnapshot;
  const state = { url: '/rooms' } as RouterStateSnapshot;

  let authService: jasmine.SpyObj<AuthService>;

  beforeEach(() => {
    authService = jasmine.createSpyObj<AuthService>('AuthService', ['isAuthenticated']);
    TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      providers: [{ provide: AuthService, useValue: authService }]
    });
  });

  it('should allow authenticated users', () => {
    authService.isAuthenticated.and.returnValue(true);

    expect(executeGuard(route, state)).toBeTrue();
  });

  it('should redirect anonymous users to the login page with return url', () => {
    authService.isAuthenticated.and.returnValue(false);

    const result = executeGuard(route, state) as UrlTree;

    expect(TestBed.inject(Router).serializeUrl(result)).toBe('/login?returnUrl=%2Frooms');
  });
});
