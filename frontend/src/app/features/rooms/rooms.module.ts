import { NgModule } from '@angular/core';

import { SharedModule } from 'src/app/shared/shared.module';
import { RoomsRoutingModule } from './rooms-routing.module';
import { RoomListComponent } from './room-list/room-list.component';
import { RoomFormDialogComponent } from './room-form-dialog/room-form-dialog.component';

@NgModule({
  declarations: [
    RoomListComponent,
    RoomFormDialogComponent
  ],
  imports: [
    SharedModule,
    RoomsRoutingModule
  ]
})
export class RoomsModule { }
