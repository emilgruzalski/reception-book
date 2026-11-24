import { Component, OnInit } from '@angular/core';
import { BreakpointObserver } from '@angular/cdk/layout';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { catchError, EMPTY, finalize, forkJoin, Subject, switchMap } from 'rxjs';

import { Dashboard } from 'src/app/core/models/dashboard.model';
import { Reservation } from 'src/app/core/models/reservation.model';
import { Room } from 'src/app/core/models/room.model';
import { DashboardService } from 'src/app/core/services/dashboard.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { ReservationService } from 'src/app/core/services/reservation.service';
import { RoomService } from 'src/app/core/services/room.service';
import { toApiDate } from 'src/app/core/utils/date.utils';
import { fetchAllPages, MAX_PAGE_SIZE } from 'src/app/core/utils/paging.utils';
import { addDays, startOfDay } from './stay-chart/stay-chart.utils';

const HANDSET = '(max-width: 599.98px)';
const DAYS = 14;
const DAYS_HANDSET = 7;
/** Days before today that are visible when the chart opens. */
const DAYS_BEFORE_TODAY = 2;
const DAYS_PER_STEP = 7;

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  today = startOfDay(new Date());

  dashboard?: Dashboard;
  rooms: Room[] = [];
  reservations: Reservation[] = [];
  start = addDays(this.today, -DAYS_BEFORE_TODAY);
  dayCount = DAYS;
  isLoading = false;

  private readonly reload$ = new Subject<void>();

  constructor(
    private dashboardService: DashboardService,
    private reservationService: ReservationService,
    private roomService: RoomService,
    private notificationService: NotificationService,
    breakpointObserver: BreakpointObserver
  ) {
    // Only the latest range matters when the user moves through the weeks quickly.
    this.reload$.pipe(
      switchMap(() => this.load()),
      takeUntilDestroyed()
    ).subscribe(({ dashboard, rooms, reservations }) => {
      this.dashboard = dashboard;
      this.rooms = rooms;
      this.reservations = reservations;
    });

    breakpointObserver.observe(HANDSET)
      .pipe(takeUntilDestroyed())
      .subscribe(result => this.dayCount = result.matches ? DAYS_HANDSET : DAYS);
  }

  get end(): Date {
    return addDays(this.start, this.dayCount - 1);
  }

  get roomsToClean(): Room[] {
    return this.rooms.filter(room => room.status === 'Cleaning');
  }

  ngOnInit(): void {
    this.reload$.next();
  }

  move(direction: -1 | 1): void {
    this.start = addDays(this.start, direction * DAYS_PER_STEP);
    this.reload$.next();
  }

  showToday(): void {
    this.start = addDays(this.today, -DAYS_BEFORE_TODAY);
    this.reload$.next();
  }

  /** Reason why the guest cannot be checked in yet, or undefined when the room is ready. */
  roomNotReady(reservation: Reservation): Room['status'] | undefined {
    const status = this.rooms.find(room => room.id === reservation.roomId)?.status;
    return status === 'Available' ? undefined : status;
  }

  checkIn(reservation: Reservation): void {
    this.reservationService.checkIn(reservation.id).subscribe(() => {
      this.notificationService.success(`Zameldowano gościa w pokoju ${reservation.roomNumber}.`);
      this.reload$.next();
    });
  }

  checkOut(reservation: Reservation): void {
    this.reservationService.checkOut(reservation.id).subscribe(() => {
      this.notificationService.success(`Wymeldowano gościa z pokoju ${reservation.roomNumber}.`);
      this.reload$.next();
    });
  }

  markClean(room: Room): void {
    this.roomService.updateRoomStatus(room.id, 'Available').subscribe(() => {
      this.notificationService.success(`Pokój ${room.number} jest gotowy.`);
      this.reload$.next();
    });
  }

  private load() {
    this.isLoading = true;
    // The page can stay open past midnight.
    this.today = startOfDay(new Date());

    return forkJoin({
      dashboard: this.dashboardService.getDashboard(),
      rooms: fetchAllPages(pageNumber =>
        this.roomService.getRooms({ pageNumber, pageSize: MAX_PAGE_SIZE, orderBy: 'number' })),
      reservations: fetchAllPages(pageNumber => this.reservationService.getReservations({
        pageNumber,
        pageSize: MAX_PAGE_SIZE,
        orderBy: 'checkInDate',
        fromDate: toApiDate(this.start),
        toDate: toApiDate(addDays(this.start, DAYS - 1))
      }))
    }).pipe(
      // The error is shown by the HTTP interceptor; the next reload must still work.
      catchError(() => EMPTY),
      finalize(() => this.isLoading = false)
    );
  }
}
