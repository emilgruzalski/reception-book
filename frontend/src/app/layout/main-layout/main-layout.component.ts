import { Component, ViewChild } from '@angular/core';
import { BreakpointObserver } from '@angular/cdk/layout';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatSidenav } from '@angular/material/sidenav';

import { ADMINISTRATOR_ONLY, UserRoles } from 'src/app/core/models/auth.model';
import { AuthService } from 'src/app/core/services/auth.service';

/** Below this width the links do not fit in the header and move to the side menu. */
const COMPACT_LAYOUT = '(max-width: 959.98px)';

const EVERYONE: readonly string[] = Object.values(UserRoles);

@Component({
  selector: 'app-main-layout',
  templateUrl: './main-layout.component.html',
  styleUrls: ['./main-layout.component.scss']
})
export class MainLayoutComponent {
  @ViewChild('sidenav') private sidenav?: MatSidenav;

  readonly links = [
    { path: '/dashboard', label: 'Grafik', icon: 'calendar_view_week', roles: EVERYONE },
    { path: '/reservations', label: 'Rezerwacje', icon: 'event', roles: EVERYONE },
    { path: '/guests', label: 'Goście', icon: 'people', roles: EVERYONE },
    { path: '/rooms', label: 'Pokoje', icon: 'meeting_room', roles: EVERYONE },
    { path: '/users', label: 'Użytkownicy', icon: 'manage_accounts', roles: ADMINISTRATOR_ONLY }
  ];
  readonly currentUser$ = this.authService.currentUser$;

  isCompact = false;

  constructor(
    private authService: AuthService,
    breakpointObserver: BreakpointObserver
  ) {
    breakpointObserver.observe(COMPACT_LAYOUT)
      .pipe(takeUntilDestroyed())
      .subscribe(result => {
        this.isCompact = result.matches;
        if (!result.matches) {
          this.sidenav?.close();
        }
      });
  }

  logout(): void {
    this.authService.logout();
  }
}
