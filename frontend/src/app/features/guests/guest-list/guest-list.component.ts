import { Component, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import { debounceTime, distinctUntilChanged, filter, finalize, switchMap } from 'rxjs';

import { SEARCH_DEBOUNCE_MS } from 'src/app/core/constants';
import { ADMINISTRATOR_ONLY } from 'src/app/core/models/auth.model';
import { Guest, GuestQuery } from 'src/app/core/models/guest.model';
import { DEFAULT_PAGE_SIZE, PAGE_SIZE_OPTIONS } from 'src/app/core/models/paged-result.model';
import { GuestService } from 'src/app/core/services/guest.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { ConfirmDialogComponent, ConfirmDialogData } from 'src/app/shared/components/confirm-dialog/confirm-dialog.component';
import { GuestFormDialogComponent, GuestFormDialogData } from '../guest-form-dialog/guest-form-dialog.component';

@Component({
  selector: 'app-guest-list',
  templateUrl: './guest-list.component.html'
})
export class GuestListComponent implements OnInit {
  readonly administratorOnly = ADMINISTRATOR_ONLY;
  readonly displayedColumns = ['name', 'documentNumber', 'email', 'phoneNumber', 'createdAt', 'actions'];

  readonly pageSizeOptions = PAGE_SIZE_OPTIONS;
  readonly searchControl = new FormControl('', { nonNullable: true });

  guests: Guest[] = [];
  totalCount = 0;
  query: GuestQuery = { pageNumber: 1, pageSize: DEFAULT_PAGE_SIZE };
  isLoading = false;

  constructor(
    private guestService: GuestService,
    private dialog: MatDialog,
    private notificationService: NotificationService
  ) {
    this.searchControl.valueChanges.pipe(
      debounceTime(SEARCH_DEBOUNCE_MS),
      distinctUntilChanged(),
      takeUntilDestroyed()
    ).subscribe(searchTerm => {
      this.query = { ...this.query, searchTerm };
      this.loadGuests();
    });
  }

  ngOnInit(): void {
    this.loadGuests();
  }

  loadGuests(): void {
    this.isLoading = true;
    this.guestService.getGuests(this.query)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe(result => {
        this.guests = result.items;
        this.totalCount = result.pagination.totalCount;
      });
  }

  onPageChange(event: PageEvent): void {
    this.query = { ...this.query, pageNumber: event.pageIndex + 1, pageSize: event.pageSize };
    this.loadGuests();
  }

  openGuestDialog(guest?: Guest): void {
    this.dialog.open<GuestFormDialogComponent, GuestFormDialogData, boolean>(GuestFormDialogComponent, {
      width: '600px',
      data: { guest }
    }).afterClosed().subscribe(saved => {
      if (saved) {
        this.notificationService.success('Zapisano dane gościa.');
        this.loadGuests();
      }
    });
  }

  deleteGuest(guest: Guest): void {
    this.dialog.open<ConfirmDialogComponent, ConfirmDialogData, boolean>(ConfirmDialogComponent, {
      data: {
        title: 'Usuń gościa',
        message: `Czy na pewno chcesz usunąć gościa ${guest.firstName} ${guest.lastName}?`
      }
    }).afterClosed().pipe(
      filter(confirmed => !!confirmed),
      switchMap(() => this.guestService.deleteGuest(guest.id))
    ).subscribe(() => {
      this.notificationService.success('Usunięto gościa.');
      this.loadGuests();
    });
  }
}
