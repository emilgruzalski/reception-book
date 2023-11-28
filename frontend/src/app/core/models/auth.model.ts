export const UserRoles = {
  Administrator: 'Administrator',
  Receptionist: 'Receptionist'
} as const;

export type UserRole = typeof UserRoles[keyof typeof UserRoles];

export interface LoginRequest {
  userName: string;
  password: string;
}

export interface TokenResponse {
  accessToken: string;
  refreshToken: string;
}

export interface CurrentUser {
  userName: string;
  firstName: string;
  lastName: string;
  roles: string[];
}
