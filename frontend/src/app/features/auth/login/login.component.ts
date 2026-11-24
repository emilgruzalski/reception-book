import { Component } from '@angular/core';
import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs';

import { AuthService } from 'src/app/core/services/auth.service';

const DEFAULT_RETURN_URL = '/dashboard';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  readonly form = this.fb.nonNullable.group({
    userName: ['', Validators.required],
    password: ['', Validators.required]
  });

  hidePassword = true;
  isSubmitting = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  login(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.errorMessage = '';
    this.isSubmitting = true;

    this.authService.login(this.form.getRawValue())
      .pipe(finalize(() => this.isSubmitting = false))
      .subscribe({
        next: () => this.router.navigateByUrl(this.getReturnUrl()),
        error: (error: HttpErrorResponse) => this.errorMessage = this.getErrorMessage(error)
      });
  }

  /** Only allow redirects to local paths to prevent open redirect attacks. */
  private getReturnUrl(): string {
    const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
    return returnUrl?.startsWith('/') && !returnUrl.startsWith('//') ? returnUrl : DEFAULT_RETURN_URL;
  }

  private getErrorMessage(error: HttpErrorResponse): string {
    switch (error.status) {
      case HttpStatusCode.Unauthorized:
        return 'Nieprawidłowa nazwa użytkownika lub hasło.';
      case HttpStatusCode.TooManyRequests:
        return 'Zbyt wiele prób logowania. Spróbuj ponownie za chwilę.';
      default:
        return 'Nie udało się zalogować. Spróbuj ponownie później.';
    }
  }
}
