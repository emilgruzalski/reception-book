import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { MaterialModule } from './material.module';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { RoomStatusPipe } from './pipes/room-status.pipe';
import { RoomTypePipe } from './pipes/room-type.pipe';

@NgModule({
  declarations: [
    ConfirmDialogComponent,
    RoomStatusPipe,
    RoomTypePipe
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MaterialModule
  ],
  exports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MaterialModule,
    RoomStatusPipe,
    RoomTypePipe
  ]
})
export class SharedModule { }
