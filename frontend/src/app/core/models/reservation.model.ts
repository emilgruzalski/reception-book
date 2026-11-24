export type ReservationStatus = 'Confirmed' | 'CheckedIn' | 'CheckedOut' | 'Cancelled';

export const RESERVATION_STATUSES: ReservationStatus[] = ['Confirmed', 'CheckedIn', 'CheckedOut', 'Cancelled'];

export interface Reservation {
  id: string;
  roomId: string;
  roomNumber: string;
  guestId: string;
  guestFullName: string;
  checkInDate: string;
  checkOutDate: string;
  numberOfGuests: number;
  status: ReservationStatus;
  totalPrice: number;
  notes?: string;
  createdAt: string;
}

export interface ReservationForManipulation {
  roomId: string;
  guestId: string;
  checkInDate: string;
  checkOutDate: string;
  numberOfGuests: number;
  notes?: string;
}

export interface ReservationQuery {
  pageNumber: number;
  pageSize: number;
  orderBy?: string;
  searchTerm?: string;
  status?: ReservationStatus;
  fromDate?: string;
  toDate?: string;
}
