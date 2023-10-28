import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { MaterialModule } from './material.module';
import { RoomStatusPipe } from './pipes/room-status.pipe';
import { RoomTypePipe } from './pipes/room-type.pipe';

@NgModule({
  declarations: [
    RoomStatusPipe,
    RoomTypePipe
  ],
  imports: [
    CommonModule,
    RouterModule,
    MaterialModule
  ],
  exports: [
    CommonModule,
    RouterModule,
    MaterialModule,
    RoomStatusPipe,
    RoomTypePipe
  ]
})
export class SharedModule { }
