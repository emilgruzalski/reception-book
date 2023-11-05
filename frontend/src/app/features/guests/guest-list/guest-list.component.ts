import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { filter, finalize, switchMap } from 'rxjs';

import { Guest } from 'src/app/core/models/guest.model';
import { GuestService } from 'src/app/core/services/guest.service';
import { ConfirmDialogComponent, ConfirmDialogData } from 'src/app/shared/components/confirm-dialog/confirm-dialog.component';
import { GuestFormDialogComponent, GuestFormDialogData } from '../guest-form-dialog/guest-form-dialog.component';

@Component({
  selector: 'app-guest-list',
  templateUrl: './guest-list.component.html'
})
export class GuestListComponent implements OnInit {
  readonly displayedColumns = ['name', 'documentNumber', 'email', 'phoneNumber', 'createdAt', 'actions'];

  guests: Guest[] = [];
  isLoading = false;

  constructor(
    private guestService: GuestService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadGuests();
  }

  loadGuests(): void {
    this.isLoading = true;
    this.guestService.getGuests()
      .pipe(finalize(() => this.isLoading = false))
      .subscribe(guests => this.guests = guests);
  }

  openGuestDialog(guest?: Guest): void {
    this.dialog.open<GuestFormDialogComponent, GuestFormDialogData, boolean>(GuestFormDialogComponent, {
      width: '600px',
      data: { guest }
    }).afterClosed().subscribe(saved => {
      if (saved) {
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
    ).subscribe(() => this.loadGuests());
  }
}
