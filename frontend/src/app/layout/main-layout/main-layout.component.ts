import { Component } from '@angular/core';

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

  constructor(private authService: AuthService) { }

  logout(): void {
    this.authService.logout();
  }
}
