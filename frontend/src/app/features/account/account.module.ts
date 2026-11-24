import { NgModule } from '@angular/core';

import { SharedModule } from 'src/app/shared/shared.module';
import { AccountRoutingModule } from './account-routing.module';
import { ChangePasswordComponent } from './change-password/change-password.component';

@NgModule({
  declarations: [
    ChangePasswordComponent
  ],
  imports: [
    SharedModule,
    AccountRoutingModule
  ]
})
export class AccountModule { }
