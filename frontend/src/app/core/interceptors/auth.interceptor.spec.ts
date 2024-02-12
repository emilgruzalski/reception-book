import { TestBed } from '@angular/core/testing';
import { HTTP_INTERCEPTORS, HttpClient, HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';

import { environment } from 'src/environments/environment';
import { AuthService } from '../services/auth.service';
import { AuthInterceptor } from './auth.interceptor';

describe('AuthInterceptor', () => {
  const roomsUrl = `${environment.apiUrl}/rooms`;
  const unauthorized = { status: HttpStatusCode.Unauthorized, statusText: 'Unauthorized' };

  let http: HttpClient;
  let httpMock: HttpTestingController;
  let authService: jasmine.SpyObj<AuthService>;

  beforeEach(() => {
    authService = jasmine.createSpyObj<AuthService>('AuthService',
      ['getAccessToken', 'refreshToken', 'endSession', 'isAuthRequest']);
    authService.getAccessToken.and.returnValue('access-token');
    authService.isAuthRequest.and.returnValue(false);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        { provide: AuthService, useValue: authService },
        { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
      ]
    });

    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should add the access token to API requests', () => {
    http.get(roomsUrl).subscribe();

    const request = httpMock.expectOne(roomsUrl);
    expect(request.request.headers.get('Authorization')).toBe('Bearer access-token');
    request.flush([]);
  });

  it('should not send the access token to other hosts', () => {
    http.get('https://example.com/data').subscribe();

    const request = httpMock.expectOne('https://example.com/data');
    expect(request.request.headers.has('Authorization')).toBeFalse();
    request.flush({});
  });

  it('should refresh the token and retry the request after 401', () => {
    authService.refreshToken.and.returnValue(of({ accessToken: 'new-access-token', refreshToken: 'new-refresh-token' }));
    let response: unknown;

    http.get(roomsUrl).subscribe(body => response = body);
    httpMock.expectOne(roomsUrl).flush(null, unauthorized);

    const retried = httpMock.expectOne(roomsUrl);
    expect(retried.request.headers.get('Authorization')).toBe('Bearer new-access-token');
    retried.flush([{ id: '1' }]);
    expect(response).toEqual([{ id: '1' }]);
  });

  it('should end the session when the token cannot be refreshed', () => {
    authService.refreshToken.and.returnValue(throwError(() => new HttpErrorResponse({ status: HttpStatusCode.BadRequest })));

    http.get(roomsUrl).subscribe({ error: () => undefined });
    httpMock.expectOne(roomsUrl).flush(null, unauthorized);

    expect(authService.endSession).toHaveBeenCalled();
  });

  it('should not refresh the token when login fails', () => {
    authService.isAuthRequest.and.returnValue(true);

    http.post(`${environment.apiUrl}/authentication/login`, {}).subscribe({ error: () => undefined });
    httpMock.expectOne(`${environment.apiUrl}/authentication/login`).flush(null, unauthorized);

    expect(authService.refreshToken).not.toHaveBeenCalled();
  });
});
