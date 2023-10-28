export type RoomType = 'Single' | 'Double' | 'Twin' | 'Triple' | 'Suite' | 'Apartment';

export type RoomStatus = 'Available' | 'Occupied' | 'Cleaning' | 'OutOfService';

export const ROOM_TYPES: RoomType[] = ['Single', 'Double', 'Twin', 'Triple', 'Suite', 'Apartment'];

export const ROOM_STATUSES: RoomStatus[] = ['Available', 'Occupied', 'Cleaning', 'OutOfService'];

export interface Room {
  id: string;
  number: string;
  floor: number;
  type: RoomType;
  capacity: number;
  pricePerNight: number;
  status: RoomStatus;
  description?: string;
}

export interface RoomForManipulation {
  number: string;
  floor: number;
  type: RoomType;
  capacity: number;
  pricePerNight: number;
  description?: string;
  status?: RoomStatus;
}
