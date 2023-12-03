import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { BehaviorSubject, map, Observable, tap, throwError } from 'rxjs';

import { environment } from 'src/environments/environment';
import { CurrentUser, LoginRequest, TokenResponse } from '../models/auth.model';

const ACCESS_TOKEN_KEY = 'accessToken';
const REFRESH_TOKEN_KEY = 'refreshToken';

/** Claim types used by ASP.NET Core Identity in the access token. */
const CLAIMS = {
  name: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name',
  givenName: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname',
  surname: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname',
  role: 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
};

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly url = `${environment.apiUrl}/authentication`;
  private readonly tokenUrl = `${environment.apiUrl}/token`;
  private readonly jwtHelper = new JwtHelperService();
  private readonly currentUserSubject = new BehaviorSubject<CurrentUser | null>(this.readCurrentUser());

  readonly currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) { }

  get currentUser(): CurrentUser | null {
    return this.currentUserSubject.value;
  }

  login(credentials: LoginRequest): Observable<void> {
    return this.http.post<TokenResponse>(`${this.url}/login`, credentials).pipe(
      tap(tokens => this.storeTokens(tokens)),
      map(() => undefined)
    );
  }

  refreshToken(): Observable<TokenResponse> {
    const accessToken = this.getAccessToken();
    const refreshToken = this.getRefreshToken();
    if (!accessToken || !refreshToken) {
      return throwError(() => new Error('No tokens to refresh.'));
    }

    return this.http.post<TokenResponse>(`${this.tokenUrl}/refresh`, { accessToken, refreshToken }).pipe(
      tap(tokens => this.storeTokens(tokens))
    );
  }

  logout(): void {
    this.clearTokens();
    this.router.navigate(['/login']);
  }

  /** An expired access token is fine as long as there is a refresh token to renew it. */
  isAuthenticated(): boolean {
    return !!this.getAccessToken() && !!this.getRefreshToken();
  }

  hasAnyRole(roles: readonly string[]): boolean {
    const userRoles = this.currentUser?.roles ?? [];
    return roles.some(role => userRoles.includes(role));
  }

  getAccessToken(): string | null {
    return localStorage.getItem(ACCESS_TOKEN_KEY);
  }

  isAuthRequest(url: string): boolean {
    return url.startsWith(`${this.url}/login`) || url.startsWith(`${this.tokenUrl}/refresh`);
  }

  private getRefreshToken(): string | null {
    return localStorage.getItem(REFRESH_TOKEN_KEY);
  }

  private storeTokens(tokens: TokenResponse): void {
    localStorage.setItem(ACCESS_TOKEN_KEY, tokens.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, tokens.refreshToken);
    this.currentUserSubject.next(this.readCurrentUser());
  }

  private clearTokens(): void {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    this.currentUserSubject.next(null);
  }

  private readCurrentUser(): CurrentUser | null {
    const token = this.getAccessToken();
    if (!token) {
      return null;
    }

    try {
      const payload = this.jwtHelper.decodeToken(token);
      const roles = payload[CLAIMS.role];

      return {
        userName: payload[CLAIMS.name],
        firstName: payload[CLAIMS.givenName],
        lastName: payload[CLAIMS.surname],
        roles: Array.isArray(roles) ? roles : roles ? [roles] : []
      };
    } catch {
      return null;
    }
  }
}
