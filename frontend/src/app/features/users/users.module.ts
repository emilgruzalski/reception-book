import { NgModule } from '@angular/core';

import { SharedModule } from 'src/app/shared/shared.module';
import { UsersRoutingModule } from './users-routing.module';
import { UserListComponent } from './user-list/user-list.component';
import { UserFormDialogComponent } from './user-form-dialog/user-form-dialog.component';

@NgModule({
  declarations: [
    UserListComponent,
    UserFormDialogComponent
  ],
  imports: [
    SharedModule,
    UsersRoutingModule
  ]
})
export class UsersModule { }
