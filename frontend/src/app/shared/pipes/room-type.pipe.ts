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

@Pipe({
  name: 'roomType'
})
export class RoomTypePipe implements PipeTransform {
  transform(value: RoomType | null | undefined): string {
    return value ? ROOM_TYPE_LABELS[value] ?? value : '';
  }
}
