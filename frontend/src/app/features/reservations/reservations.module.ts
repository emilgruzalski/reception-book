import { NgModule } from '@angular/core';

import { SharedModule } from 'src/app/shared/shared.module';
import { ReservationsRoutingModule } from './reservations-routing.module';
import { ReservationListComponent } from './reservation-list/reservation-list.component';
import { ReservationFormComponent } from './reservation-form/reservation-form.component';

@NgModule({
  declarations: [
    ReservationListComponent,
    ReservationFormComponent
  ],
  imports: [
    SharedModule,
    ReservationsRoutingModule
  ]
})
export class ReservationsModule { }
