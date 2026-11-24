export interface User {
  id: string;
  userName: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  roles: string[];
  isLockedOut: boolean;
}

export interface UserForUpdate {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  roles: string[];
}

export interface UserForRegistration extends UserForUpdate {
  userName: string;
  password: string;
}
