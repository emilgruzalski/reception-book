import { Component, Inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { finalize, Observable } from 'rxjs';

import { Guest } from 'src/app/core/models/guest.model';
import { GuestService } from 'src/app/core/services/guest.service';

export interface GuestFormDialogData {
  guest?: Guest;
}

@Component({
  selector: 'app-guest-form-dialog',
  templateUrl: './guest-form-dialog.component.html'
})
export class GuestFormDialogComponent {
  readonly isEditMode: boolean;

  readonly form = this.fb.nonNullable.group({
    firstName: ['', [Validators.required, Validators.maxLength(50)]],
    lastName: ['', [Validators.required, Validators.maxLength(50)]],
    documentNumber: ['', [Validators.required, Validators.maxLength(30)]],
    email: ['', [Validators.email, Validators.maxLength(100)]],
    phoneNumber: ['', Validators.maxLength(20)],
    address: ['', Validators.maxLength(200)]
  });

  isSaving = false;

  constructor(
    private fb: FormBuilder,
    private guestService: GuestService,
    private dialogRef: MatDialogRef<GuestFormDialogComponent, boolean>,
    @Inject(MAT_DIALOG_DATA) public data: GuestFormDialogData
  ) {
    this.isEditMode = !!data.guest;
    if (data.guest) {
      this.form.patchValue(data.guest);
    }
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { email, phoneNumber, address, ...required } = this.form.getRawValue();
    const guest = {
      ...required,
      email: email || undefined,
      phoneNumber: phoneNumber || undefined,
      address: address || undefined
    };

    const request$: Observable<unknown> = this.data.guest
      ? this.guestService.updateGuest(this.data.guest.id, guest)
      : this.guestService.createGuest(guest);

    this.isSaving = true;
    request$
      .pipe(finalize(() => this.isSaving = false))
      .subscribe(() => this.dialogRef.close(true));
  }
}
