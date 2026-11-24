import { Injectable } from '@angular/core';
import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpStatusCode
} from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';

import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';
import { getErrorMessage } from '../utils/error.utils';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(
    private notificationService: NotificationService,
    private authService: AuthService
  ) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: unknown) => {
        if (error instanceof HttpErrorResponse && this.shouldNotify(request, error)) {
          this.notificationService.error(getErrorMessage(error));
        }

        return throwError(() => error);
      })
    );
  }

  /** 401 responses are handled by the auth interceptor and authentication errors by the login page. */
  private shouldNotify(request: HttpRequest<unknown>, error: HttpErrorResponse): boolean {
    return error.status !== HttpStatusCode.Unauthorized && !this.authService.isAuthRequest(request.url);
  }
}
