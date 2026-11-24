import { Injectable } from '@angular/core';
import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpStatusCode
} from '@angular/common/http';
import { BehaviorSubject, catchError, filter, finalize, Observable, switchMap, take, throwError } from 'rxjs';

import { environment } from 'src/environments/environment';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private readonly refreshedToken$ = new BehaviorSubject<string | null>(null);

  constructor(private authService: AuthService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // The token is attached only to requests sent to our API, so it never leaks to other hosts.
    if (!request.url.startsWith(environment.apiUrl)) {
      return next.handle(request);
    }

    return next.handle(this.withToken(request, this.authService.getAccessToken())).pipe(
      catchError(error => {
        if (error instanceof HttpErrorResponse && error.status === HttpStatusCode.Unauthorized
          && !this.authService.isAuthRequest(request.url)) {
          return this.refreshAndRetry(request, next);
        }

        return throwError(() => error);
      })
    );
  }

  private refreshAndRetry(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Requests that fail while a refresh is in progress wait for the new token instead of refreshing again.
    if (this.isRefreshing) {
      return this.refreshedToken$.pipe(
        filter((token): token is string => token !== null),
        take(1),
        switchMap(token => next.handle(this.withToken(request, token)))
      );
    }

    this.isRefreshing = true;
    this.refreshedToken$.next(null);

    return this.authService.refreshToken().pipe(
      switchMap(tokens => {
        this.refreshedToken$.next(tokens.accessToken);
        return next.handle(this.withToken(request, tokens.accessToken));
      }),
      catchError(error => {
        this.authService.endSession();
        return throwError(() => error);
      }),
      finalize(() => this.isRefreshing = false)
    );
  }

  private withToken(request: HttpRequest<unknown>, token: string | null): HttpRequest<unknown> {
    return token ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : request;
  }
}
