import { Component, OnInit } from '@angular/core';
import { finalize } from 'rxjs';

import { Dashboard } from 'src/app/core/models/dashboard.model';
import { Reservation } from 'src/app/core/models/reservation.model';
import { DashboardService } from 'src/app/core/services/dashboard.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { ReservationService } from 'src/app/core/services/reservation.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  readonly arrivalColumns = ['roomNumber', 'guestFullName', 'nights', 'status', 'actions'];
  readonly departureColumns = ['roomNumber', 'guestFullName', 'totalPrice', 'status', 'actions'];

  dashboard?: Dashboard;
  isLoading = false;

  constructor(
    private dashboardService: DashboardService,
    private reservationService: ReservationService,
    private notificationService: NotificationService
  ) { }

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.isLoading = true;
    this.dashboardService.getDashboard()
      .pipe(finalize(() => this.isLoading = false))
      .subscribe(dashboard => this.dashboard = dashboard);
  }

  checkIn(reservation: Reservation): void {
    this.reservationService.checkIn(reservation.id).subscribe(() => {
      this.notificationService.success(`Zameldowano gościa w pokoju ${reservation.roomNumber}.`);
      this.loadDashboard();
    });
  }

  checkOut(reservation: Reservation): void {
    this.reservationService.checkOut(reservation.id).subscribe(() => {
      this.notificationService.success(`Wymeldowano gościa z pokoju ${reservation.roomNumber}.`);
      this.loadDashboard();
    });
  }
}
