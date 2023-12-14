import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { MaterialModule } from './material.module';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { HasRoleDirective } from './directives/has-role.directive';
import { ReservationStatusPipe } from './pipes/reservation-status.pipe';
import { RoomStatusPipe } from './pipes/room-status.pipe';
import { RoomTypePipe } from './pipes/room-type.pipe';
import { UserRolePipe } from './pipes/user-role.pipe';

@NgModule({
  declarations: [
    ConfirmDialogComponent,
    ReservationStatusPipe,
    RoomStatusPipe,
    RoomTypePipe,
    UserRolePipe
  ],
  imports: [
    CommonModule,
    HasRoleDirective,
    ReactiveFormsModule,
    RouterModule,
    MaterialModule
  ],
  exports: [
    CommonModule,
    HasRoleDirective,
    ReactiveFormsModule,
    RouterModule,
    MaterialModule,
    ReservationStatusPipe,
    RoomStatusPipe,
    RoomTypePipe,
    UserRolePipe
  ]
})
export class SharedModule { }
