import { ChangeDetectionStrategy, Component, Input, OnChanges } from '@angular/core';

import { Reservation } from 'src/app/core/models/reservation.model';
import { Room } from 'src/app/core/models/room.model';
import { addDays, buildFloors, ChartFloor, ChartRow, daysBetween, Stay } from './stay-chart.utils';

interface ChartDay {
  date: Date;
  isToday: boolean;
  isWeekend: boolean;
}

/** Rooms as rows, days as columns and stays as bars. */
@Component({
  selector: 'app-stay-chart',
  templateUrl: './stay-chart.component.html',
  styleUrls: ['./stay-chart.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class StayChartComponent implements OnChanges {
  @Input({ required: true }) rooms: Room[] = [];
  @Input({ required: true }) reservations: Reservation[] = [];
  @Input({ required: true }) start = new Date();
  @Input({ required: true }) today = new Date();
  @Input() dayCount = 14;

  days: ChartDay[] = [];
  floors: ChartFloor[] = [];
  todayIndex = -1;

  ngOnChanges(): void {
    this.todayIndex = daysBetween(this.start, this.today);
    this.days = Array.from({ length: this.dayCount }, (_, index) => {
      const date = addDays(this.start, index);
      return { date, isToday: index === this.todayIndex, isWeekend: [0, 6].includes(date.getDay()) };
    });
    this.floors = buildFloors(this.rooms, this.reservations, this.start, this.dayCount, this.today);
  }

  isEditable(stay: Stay): boolean {
    return stay.kind !== 'finished';
  }

  trackRow(_: number, row: ChartRow): string {
    return row.room.id;
  }

  trackStay(_: number, stay: Stay): string {
    return stay.reservation.id;
  }
}
