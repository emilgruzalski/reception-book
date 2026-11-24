import { Component, Inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { finalize, Observable } from 'rxjs';

import { PASSWORD_MIN_LENGTH, PASSWORD_PATTERN } from 'src/app/core/constants';
import { UserRoles } from 'src/app/core/models/auth.model';
import { User } from 'src/app/core/models/user.model';
import { UserService } from 'src/app/core/services/user.service';

export interface UserFormDialogData {
  user?: User;
}

@Component({
  selector: 'app-user-form-dialog',
  templateUrl: './user-form-dialog.component.html'
})
export class UserFormDialogComponent {
  readonly roles = Object.values(UserRoles);
  readonly passwordMinLength = PASSWORD_MIN_LENGTH;
  readonly isEditMode: boolean;

  readonly form = this.fb.nonNullable.group({
    firstName: ['', [Validators.required, Validators.maxLength(50)]],
    lastName: ['', [Validators.required, Validators.maxLength(50)]],
    userName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: [''],
    password: ['', [Validators.required, Validators.minLength(PASSWORD_MIN_LENGTH), Validators.pattern(PASSWORD_PATTERN)]],
    roles: [[UserRoles.Receptionist] as string[], Validators.required]
  });

  isSaving = false;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private dialogRef: MatDialogRef<UserFormDialogComponent, boolean>,
    @Inject(MAT_DIALOG_DATA) public data: UserFormDialogData
  ) {
    this.isEditMode = !!data.user;
    if (data.user) {
      this.form.patchValue(data.user);
      // User name and password cannot be changed by the administrator.
      this.form.controls.userName.disable();
      this.form.controls.password.disable();
    }
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { userName, password, phoneNumber, ...user } = this.form.getRawValue();
    const request$: Observable<unknown> = this.data.user
      ? this.userService.updateUser(this.data.user.id, { ...user, phoneNumber: phoneNumber || undefined })
      : this.userService.registerUser({ ...user, userName, password, phoneNumber: phoneNumber || undefined });

    this.isSaving = true;
    request$
      .pipe(finalize(() => this.isSaving = false))
      .subscribe(() => this.dialogRef.close(true));
  }
}
