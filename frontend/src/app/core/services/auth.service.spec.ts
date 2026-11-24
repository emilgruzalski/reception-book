import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';

import { environment } from 'src/environments/environment';
import { createTestToken } from 'src/testing/test-token';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let router: Router;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    router = TestBed.inject(Router);
    spyOn(router, 'navigate').and.resolveTo(true);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should store tokens and read the current user from claims after login', () => {
    const accessToken = createTestToken({ name: 'anowak', givenName: 'Anna', surname: 'Nowak', roles: ['Receptionist'] });

    service.login({ userName: 'anowak', password: 'secret' }).subscribe();
    httpMock.expectOne(`${environment.apiUrl}/authentication/login`).flush({ accessToken, refreshToken: 'refresh' });

    expect(service.isAuthenticated()).toBeTrue();
    expect(service.currentUser).toEqual({ userName: 'anowak', firstName: 'Anna', lastName: 'Nowak', roles: ['Receptionist'] });
  });

  it('should check roles of the current user', () => {
    const accessToken = createTestToken({
      name: 'admin', givenName: 'System', surname: 'Administrator', roles: ['Administrator', 'Receptionist']
    });

    service.login({ userName: 'admin', password: 'secret' }).subscribe();
    httpMock.expectOne(`${environment.apiUrl}/authentication/login`).flush({ accessToken, refreshToken: 'refresh' });

    expect(service.hasAnyRole(['Administrator'])).toBeTrue();
    expect(service.hasAnyRole(['Manager'])).toBeFalse();
  });

  it('should revoke the refresh token and clear the session on logout', () => {
    localStorage.setItem('accessToken', 'access');
    localStorage.setItem('refreshToken', 'refresh');

    service.logout();
    const request = httpMock.expectOne(`${environment.apiUrl}/token/revoke`);
    expect(request.request.method).toBe('POST');
    request.flush(null);

    expect(service.isAuthenticated()).toBeFalse();
    expect(localStorage.getItem('refreshToken')).toBeNull();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should fail to refresh when there are no tokens', () => {
    let failed = false;

    service.refreshToken().subscribe({ error: () => failed = true });

    expect(failed).toBeTrue();
  });

  it('should recognize requests which must not trigger a token refresh', () => {
    expect(service.isAuthRequest(`${environment.apiUrl}/authentication/login`)).toBeTrue();
    expect(service.isAuthRequest(`${environment.apiUrl}/token/refresh`)).toBeTrue();
    expect(service.isAuthRequest(`${environment.apiUrl}/rooms`)).toBeFalse();
  });
});
