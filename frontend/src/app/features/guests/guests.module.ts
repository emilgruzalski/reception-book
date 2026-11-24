import { NgModule } from '@angular/core';

import { SharedModule } from 'src/app/shared/shared.module';
import { GuestsRoutingModule } from './guests-routing.module';
import { GuestListComponent } from './guest-list/guest-list.component';
import { GuestFormDialogComponent } from './guest-form-dialog/guest-form-dialog.component';

@NgModule({
  declarations: [
    GuestListComponent,
    GuestFormDialogComponent
  ],
  imports: [
    SharedModule,
    GuestsRoutingModule
  ]
})
export class GuestsModule { }
