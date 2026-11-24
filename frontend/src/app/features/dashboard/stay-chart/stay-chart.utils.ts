import { Reservation } from 'src/app/core/models/reservation.model';
import { Room } from 'src/app/core/models/room.model';
import { fromApiDate } from 'src/app/core/utils/date.utils';

export type StayKind = 'confirmed' | 'in-house' | 'departing' | 'finished';

export interface Stay {
  reservation: Reservation;
  kind: StayKind;
  /** Full name, or only the surname when the bar is short. */
  label: string;
  /** Grid lines of the day columns; every day has two half-day columns. */
  startLine: number;
  endLine: number;
  startsBefore: boolean;
  endsAfter: boolean;
}

export interface ChartRow {
  room: Room;
  stays: Stay[];
}

export interface ChartFloor {
  floor: number;
  rows: ChartRow[];
}

const HALF_DAYS = 2;

/** Bars up to this many half-day columns have room only for the surname. */
const SHORT_STAY = 4;

export function startOfDay(date: Date): Date {
  return new Date(date.getFullYear(), date.getMonth(), date.getDate());
}

export function addDays(date: Date, days: number): Date {
  return new Date(date.getFullYear(), date.getMonth(), date.getDate() + days);
}

/** Number of calendar days between two dates; rounding hides the hour lost or gained when clocks change. */
export function daysBetween(from: Date, to: Date): number {
  const msPerDay = 24 * 60 * 60 * 1000;
  return Math.round((startOfDay(to).getTime() - startOfDay(from).getTime()) / msPerDay);
}

/**
 * Guests arrive in the afternoon and leave in the morning, so a stay starts in the middle of the first day
 * and ends in the middle of the last one. Two stays can share the day when one ends and the other begins.
 */
export function placeStay(reservation: Reservation, start: Date, dayCount: number, today: Date): Stay | null {
  const first = daysBetween(start, fromApiDate(reservation.checkInDate)) * HALF_DAYS + 1;
  const last = daysBetween(start, fromApiDate(reservation.checkOutDate)) * HALF_DAYS + 1;
  const end = dayCount * HALF_DAYS;

  if (reservation.status === 'Cancelled' || last <= 0 || first >= end) {
    return null;
  }

  const startLine = Math.max(first, 0) + 1;
  const endLine = Math.min(last, end) + 1;
  const name = reservation.guestFullName;

  return {
    reservation,
    kind: stayKind(reservation, today),
    label: endLine - startLine <= SHORT_STAY ? name.substring(name.indexOf(' ') + 1) : name,
    startLine,
    endLine,
    startsBefore: first < 0,
    endsAfter: last > end
  };
}

export function buildFloors(rooms: Room[], reservations: Reservation[], start: Date, dayCount: number,
                            today: Date): ChartFloor[] {
  const floors = new Map<number, ChartRow[]>();
  const sorted = [...rooms].sort((a, b) =>
    a.floor - b.floor || a.number.localeCompare(b.number, 'pl', { numeric: true }));

  for (const room of sorted) {
    const stays = reservations
      .filter(reservation => reservation.roomId === room.id)
      .map(reservation => placeStay(reservation, start, dayCount, today))
      .filter((stay): stay is Stay => stay !== null)
      .sort((a, b) => a.startLine - b.startLine);

    floors.set(room.floor, [...(floors.get(room.floor) ?? []), { room, stays }]);
  }

  return [...floors].map(([floor, rows]) => ({ floor, rows }));
}

function stayKind(reservation: Reservation, today: Date): StayKind {
  switch (reservation.status) {
    case 'CheckedIn':
      return daysBetween(today, fromApiDate(reservation.checkOutDate)) <= 0 ? 'departing' : 'in-house';
    case 'CheckedOut':
      return 'finished';
    default:
      return 'confirmed';
  }
}
