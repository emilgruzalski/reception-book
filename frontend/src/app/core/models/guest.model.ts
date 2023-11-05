export interface Guest {
  id: string;
  firstName: string;
  lastName: string;
  email?: string;
  phoneNumber?: string;
  documentNumber: string;
  address?: string;
  createdAt: string;
}

export interface GuestForManipulation {
  firstName: string;
  lastName: string;
  email?: string;
  phoneNumber?: string;
  documentNumber: string;
  address?: string;
}
