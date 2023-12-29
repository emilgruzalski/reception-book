import { Reservation } from './reservation.model';

export interface Dashboard {
  date: string;
  totalRooms: number;
  availableRooms: number;
  occupiedRooms: number;
  roomsToClean: number;
  outOfServiceRooms: number;
  occupancyRate: number;
  guestsInHouse: number;
  arrivalsToday: Reservation[];
  departuresToday: Reservation[];
}
