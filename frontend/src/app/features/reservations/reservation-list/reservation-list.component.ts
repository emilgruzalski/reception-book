import { Component, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import { Sort } from '@angular/material/sort';
import { debounceTime, filter, finalize, switchMap } from 'rxjs';

import { SEARCH_DEBOUNCE_MS } from 'src/app/core/constants';
import { DEFAULT_PAGE_SIZE, PAGE_SIZE_OPTIONS } from 'src/app/core/models/paged-result.model';
import {
  Reservation,
  RESERVATION_STATUSES,
  ReservationQuery,
  ReservationStatus
} from 'src/app/core/models/reservation.model';
import { ReservationService } from 'src/app/core/services/reservation.service';
import { toApiDate } from 'src/app/core/utils/date.utils';
import { ConfirmDialogComponent, ConfirmDialogData } from 'src/app/shared/components/confirm-dialog/confirm-dialog.component';

const DEFAULT_ORDER_BY = 'checkInDate desc';

@Component({
  selector: 'app-reservation-list',
  templateUrl: './reservation-list.component.html'
})
export class ReservationListComponent implements OnInit {
  readonly displayedColumns = [
    'guestFullName', 'roomNumber', 'checkInDate', 'checkOutDate', 'numberOfGuests', 'totalPrice', 'status', 'actions'
  ];
  readonly pageSizeOptions = PAGE_SIZE_OPTIONS;
  readonly reservationStatuses = RESERVATION_STATUSES;

  readonly filtersForm = this.fb.group({
    searchTerm: [''],
    status: [null as ReservationStatus | null],
    fromDate: [null as Date | null],
    toDate: [null as Date | null]
  });

  reservations: Reservation[] = [];
  totalCount = 0;
  query: ReservationQuery = { pageNumber: 1, pageSize: DEFAULT_PAGE_SIZE, orderBy: DEFAULT_ORDER_BY };
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private reservationService: ReservationService,
    private dialog: MatDialog
  ) {
    this.filtersForm.valueChanges.pipe(
      debounceTime(SEARCH_DEBOUNCE_MS),
      takeUntilDestroyed()
    ).subscribe(filters => {
      this.query = {
        ...this.query,
        searchTerm: filters.searchTerm ?? undefined,
        status: filters.status ?? undefined,
        fromDate: filters.fromDate ? toApiDate(filters.fromDate) : undefined,
        toDate: filters.toDate ? toApiDate(filters.toDate) : undefined
      };
      this.loadReservations();
    });
  }

  ngOnInit(): void {
    this.loadReservations();
  }

  loadReservations(): void {
    this.isLoading = true;
    this.reservationService.getReservations(this.query)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe(result => {
        this.reservations = result.items;
        this.totalCount = result.pagination.totalCount;
      });
  }

  onPageChange(event: PageEvent): void {
    this.query = { ...this.query, pageNumber: event.pageIndex + 1, pageSize: event.pageSize };
    this.loadReservations();
  }

  onSortChange(sort: Sort): void {
    const orderBy = sort.direction ? `${sort.active} ${sort.direction}` : DEFAULT_ORDER_BY;
    this.query = { ...this.query, orderBy };
    this.loadReservations();
  }

  deleteReservation(reservation: Reservation): void {
    this.dialog.open<ConfirmDialogComponent, ConfirmDialogData, boolean>(ConfirmDialogComponent, {
      data: {
        title: 'Usuń rezerwację',
        message: `Czy na pewno chcesz usunąć rezerwację pokoju ${reservation.roomNumber} dla ${reservation.guestFullName}?`
      }
    }).afterClosed().pipe(
      filter(confirmed => !!confirmed),
      switchMap(() => this.reservationService.deleteReservation(reservation.id))
    ).subscribe(() => this.loadReservations());
  }
}
