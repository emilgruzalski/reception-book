import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { ADMINISTRATOR_ONLY } from './core/models/auth.model';

const routes: Routes = [
  {
    path: 'login',
    loadChildren: () => import('./features/auth/auth.module').then(m => m.AuthModule)
  },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: DashboardComponent },
      {
        path: 'rooms',
        loadChildren: () => import('./features/rooms/rooms.module').then(m => m.RoomsModule)
      },
      {
        path: 'guests',
        loadChildren: () => import('./features/guests/guests.module').then(m => m.GuestsModule)
      },
      {
        path: 'reservations',
        loadChildren: () => import('./features/reservations/reservations.module').then(m => m.ReservationsModule)
      },
      {
        path: 'users',
        canActivate: [roleGuard],
        data: { roles: ADMINISTRATOR_ONLY },
        loadChildren: () => import('./features/users/users.module').then(m => m.UsersModule)
      }
    ]
  },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
