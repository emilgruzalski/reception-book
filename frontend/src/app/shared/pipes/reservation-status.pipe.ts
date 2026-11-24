import { Pipe, PipeTransform } from '@angular/core';

import { ReservationStatus } from 'src/app/core/models/reservation.model';

const RESERVATION_STATUS_LABELS: Record<ReservationStatus, string> = {
  Confirmed: 'Potwierdzona',
  CheckedIn: 'Zameldowany',
  CheckedOut: 'Wymeldowany',
  Cancelled: 'Anulowana'
};

@Pipe({
  name: 'reservationStatus'
})
export class ReservationStatusPipe implements PipeTransform {
  transform(value: ReservationStatus | null | undefined): string {
    return value ? RESERVATION_STATUS_LABELS[value] ?? value : '';
  }
}
