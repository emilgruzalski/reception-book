import { Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { finalize } from 'rxjs';

import { PASSWORD_MIN_LENGTH, PASSWORD_PATTERN } from 'src/app/core/constants';
import { AuthService } from 'src/app/core/services/auth.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { matchFieldsValidator } from 'src/app/shared/validators/match-fields.validator';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent {
  readonly passwordMinLength = PASSWORD_MIN_LENGTH;

  readonly form = this.fb.nonNullable.group({
    currentPassword: ['', Validators.required],
    newPassword: ['', [Validators.required, Validators.minLength(PASSWORD_MIN_LENGTH), Validators.pattern(PASSWORD_PATTERN)]],
    confirmNewPassword: ['', Validators.required]
  }, { validators: matchFieldsValidator('newPassword', 'confirmNewPassword') });

  isSaving = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private notificationService: NotificationService
  ) { }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    this.authService.changePassword(this.form.getRawValue())
      .pipe(finalize(() => this.isSaving = false))
      .subscribe(() => {
        // The API revokes the refresh token, so the user has to sign in again with the new password.
        this.notificationService.success('Hasło zostało zmienione. Zaloguj się ponownie.');
        this.authService.endSession();
      });
  }
}
