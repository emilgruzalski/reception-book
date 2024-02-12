export interface TestTokenClaims {
  name: string;
  givenName: string;
  surname: string;
  roles: string[];
}

const ONE_HOUR_IN_SECONDS = 3600;

function base64Url(value: object): string {
  return btoa(JSON.stringify(value)).replace(/=+$/, '').replace(/\+/g, '-').replace(/\//g, '_');
}

/** Creates an unsigned JWT with the same claim types as the ASP.NET Core API. */
export function createTestToken(claims: TestTokenClaims): string {
  const payload = {
    'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name': claims.name,
    'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname': claims.givenName,
    'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname': claims.surname,
    'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': claims.roles.length === 1 ? claims.roles[0] : claims.roles,
    exp: Math.floor(Date.now() / 1000) + ONE_HOUR_IN_SECONDS
  };

  return `${base64Url({ alg: 'HS256', typ: 'JWT' })}.${base64Url(payload)}.signature`;
}
