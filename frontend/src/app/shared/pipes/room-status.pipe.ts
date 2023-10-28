import { Pipe, PipeTransform } from '@angular/core';

import { RoomStatus } from 'src/app/core/models/room.model';

const ROOM_STATUS_LABELS: Record<RoomStatus, string> = {
  Available: 'Wolny',
  Occupied: 'Zajęty',
  Cleaning: 'Do sprzątania',
  OutOfService: 'Wyłączony z użytku'
};

@Pipe({
  name: 'roomStatus'
})
export class RoomStatusPipe implements PipeTransform {
  transform(value: RoomStatus | null | undefined): string {
    return value ? ROOM_STATUS_LABELS[value] ?? value : '';
  }
}
