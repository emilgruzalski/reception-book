import { DestroyRef, Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { AuthService } from 'src/app/core/services/auth.service';

/**
 * Renders the element only for users with one of the given roles.
 * This only hides UI elements - the API enforces the same rules on the server.
 */
@Directive({
  selector: '[appHasRole]',
  standalone: true
})
export class HasRoleDirective implements OnInit {
  @Input({ required: true }) appHasRole: readonly string[] = [];

  private hasView = false;

  constructor(
    private templateRef: TemplateRef<unknown>,
    private viewContainer: ViewContainerRef,
    private authService: AuthService,
    private destroyRef: DestroyRef
  ) { }

  ngOnInit(): void {
    this.authService.currentUser$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.updateView());
  }

  private updateView(): void {
    const isAllowed = this.authService.hasAnyRole(this.appHasRole);

    if (isAllowed && !this.hasView) {
      this.viewContainer.createEmbeddedView(this.templateRef);
      this.hasView = true;
    } else if (!isAllowed && this.hasView) {
      this.viewContainer.clear();
      this.hasView = false;
    }
  }
}
