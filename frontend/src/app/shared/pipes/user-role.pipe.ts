import { Pipe, PipeTransform } from '@angular/core';

import { UserRole } from 'src/app/core/models/auth.model';

const USER_ROLE_LABELS: Record<UserRole, string> = {
  Administrator: 'Administrator',
  Receptionist: 'Recepcjonista'
};

@Pipe({
  name: 'userRole'
})
export class UserRolePipe implements PipeTransform {
  transform(value: string | null | undefined): string {
    return value ? USER_ROLE_LABELS[value as UserRole] ?? value : '';
  }
}
