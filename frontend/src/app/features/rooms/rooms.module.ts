import { NgModule } from '@angular/core';

import { SharedModule } from 'src/app/shared/shared.module';
import { RoomsRoutingModule } from './rooms-routing.module';
import { RoomListComponent } from './room-list/room-list.component';

@NgModule({
  declarations: [
    RoomListComponent
  ],
  imports: [
    SharedModule,
    RoomsRoutingModule
  ]
})
export class RoomsModule { }
