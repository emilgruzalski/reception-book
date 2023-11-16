import { NgModule } from '@angular/core';

import { SharedModule } from 'src/app/shared/shared.module';
import { ReservationsRoutingModule } from './reservations-routing.module';
import { ReservationListComponent } from './reservation-list/reservation-list.component';

@NgModule({
  declarations: [
    ReservationListComponent
  ],
  imports: [
    SharedModule,
    ReservationsRoutingModule
  ]
})
export class ReservationsModule { }
