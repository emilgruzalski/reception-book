import { Reservation, ReservationStatus } from 'src/app/core/models/reservation.model';
import { Room } from 'src/app/core/models/room.model';
import { addDays, buildFloors, daysBetween, placeStay } from './stay-chart.utils';

describe('stay chart utils', () => {
  const start = new Date(2026, 10, 22);
  const today = new Date(2026, 10, 24);

  const reservation = (checkInDate: string, checkOutDate: string, status: ReservationStatus = 'Confirmed',
                       roomId = 'r1'): Reservation =>
    ({
      id: checkInDate, roomId, roomNumber: '101', guestId: 'g', guestFullName: 'Gość', checkInDate, checkOutDate,
      numberOfGuests: 1, status, totalPrice: 0, createdAt: ''
    });

  const room = (id: string, number: string, floor: number): Room =>
    ({ id, number, floor, type: 'Single', capacity: 1, pricePerNight: 1, status: 'Available' });

  it('should start and end a stay in the middle of the day', () => {
    const stay = placeStay(reservation('2026-11-24', '2026-11-26'), start, 14, today);

    // Days 2 and 4, two half-day columns each; lines are counted from 1.
    expect(stay).toEqual(jasmine.objectContaining({ startLine: 6, endLine: 10, startsBefore: false, endsAfter: false }));
  });

  it('should let one stay end and another begin on the same day', () => {
    const first = placeStay(reservation('2026-11-23', '2026-11-25'), start, 14, today)!;
    const second = placeStay(reservation('2026-11-25', '2026-11-27'), start, 14, today)!;

    expect(second.startLine).toBe(first.endLine);
  });

  it('should cut stays that begin before or end after the visible days', () => {
    const stay = placeStay(reservation('2026-11-01', '2026-12-31'), start, 14, today);

    expect(stay).toEqual(jasmine.objectContaining({ startLine: 1, endLine: 29, startsBefore: true, endsAfter: true }));
  });

  it('should skip cancelled stays and stays outside the visible days', () => {
    expect(placeStay(reservation('2026-11-24', '2026-11-26', 'Cancelled'), start, 14, today)).toBeNull();
    expect(placeStay(reservation('2026-11-10', '2026-11-21'), start, 14, today)).toBeNull();
    expect(placeStay(reservation('2026-12-06', '2026-12-08'), start, 14, today)).toBeNull();
  });

  it('should mark guests who leave today or are overdue as departing', () => {
    const kind = (checkOut: string) => placeStay(reservation('2026-11-20', checkOut, 'CheckedIn'), start, 14, today)!.kind;

    expect(kind('2026-11-24')).toBe('departing');
    expect(kind('2026-11-23')).toBe('departing');
    expect(kind('2026-11-25')).toBe('in-house');
  });

  it('should show only the surname on short bars', () => {
    const label = (checkOut: string) => placeStay(
      { ...reservation('2026-11-24', checkOut), guestFullName: 'Ewa Jankowska' }, start, 14, today)!.label;

    expect(label('2026-11-25')).toBe('Jankowska');
    expect(label('2026-11-27')).toBe('Ewa Jankowska');
  });

  it('should group rooms by floor and sort them by number', () => {
    const rooms = [room('r3', '201', 2), room('r10', '110', 1), room('r2', '12', 1)];
    const floors = buildFloors(rooms, [reservation('2026-11-24', '2026-11-26', 'Confirmed', 'r3')], start, 14, today);

    expect(floors.map(f => [f.floor, f.rows.map(r => r.room.number)])).toEqual([[1, ['12', '110']], [2, ['201']]]);
    expect(floors[1].rows[0].stays.length).toBe(1);
    expect(floors[0].rows[0].stays.length).toBe(0);
  });

  it('should count days correctly when clocks change', () => {
    expect(daysBetween(new Date(2026, 9, 24), new Date(2026, 9, 26))).toBe(2);
    expect(daysBetween(new Date(2026, 2, 28), addDays(new Date(2026, 2, 28), 2))).toBe(2);
  });
});
