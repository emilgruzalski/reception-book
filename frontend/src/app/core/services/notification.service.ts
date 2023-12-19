import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

const SUCCESS_DURATION_MS = 3000;
const ERROR_DURATION_MS = 6000;

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  constructor(private snackBar: MatSnackBar) { }

  success(message: string): void {
    this.snackBar.open(message, 'OK', { duration: SUCCESS_DURATION_MS, panelClass: 'snackbar-success' });
  }

  error(message: string): void {
    this.snackBar.open(message, 'Zamknij', { duration: ERROR_DURATION_MS, panelClass: 'snackbar-error' });
  }
}
