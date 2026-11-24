import { Pipe, PipeTransform } from '@angular/core';

import { RoomType } from 'src/app/core/models/room.model';

const ROOM_TYPE_LABELS: Record<RoomType, string> = {
  Single: 'Jednoosobowy',
  Double: 'Dwuosobowy (łóżko małżeńskie)',
  Twin: 'Dwuosobowy (dwa łóżka)',
  Triple: 'Trzyosobowy',
  Suite: 'Suite',
  Apartment: 'Apartament'
};

const ROOM_TYPE_SHORT_LABELS: Record<RoomType, string> = {
  ...ROOM_TYPE_LABELS,
  Double: 'Dwuosobowy',
  Twin: 'Dwa łóżka'
};

@Pipe({
  name: 'roomType'
})
export class RoomTypePipe implements PipeTransform {
  transform(value: RoomType | null | undefined, format: 'full' | 'short' = 'full'): string {
    const labels = format === 'short' ? ROOM_TYPE_SHORT_LABELS : ROOM_TYPE_LABELS;
    return value ? labels[value] ?? value : '';
  }
}
