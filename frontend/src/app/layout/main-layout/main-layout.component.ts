import { Component } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatSidenav } from '@angular/material/sidenav';

import { ADMINISTRATOR_ONLY } from 'src/app/core/models/auth.model';
import { AuthService } from 'src/app/core/services/auth.service';

@Component({
  selector: 'app-main-layout',
  templateUrl: './main-layout.component.html',
  styleUrls: ['./main-layout.component.scss']
})
export class MainLayoutComponent {
  readonly administratorOnly = ADMINISTRATOR_ONLY;
  readonly currentUser$ = this.authService.currentUser$;

  isHandset = false;

  constructor(
    private authService: AuthService,
    breakpointObserver: BreakpointObserver
  ) {
    breakpointObserver.observe(Breakpoints.Handset)
      .pipe(takeUntilDestroyed())
      .subscribe(result => this.isHandset = result.matches);
  }

  closeOnHandset(sidenav: MatSidenav): void {
    if (this.isHandset) {
      sidenav.close();
    }
  }

  logout(): void {
    this.authService.logout();
  }
}
