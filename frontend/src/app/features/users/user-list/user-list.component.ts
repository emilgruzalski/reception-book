import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { filter, finalize, switchMap } from 'rxjs';

import { User } from 'src/app/core/models/user.model';
import { UserService } from 'src/app/core/services/user.service';
import { ConfirmDialogComponent, ConfirmDialogData } from 'src/app/shared/components/confirm-dialog/confirm-dialog.component';
import { UserFormDialogComponent, UserFormDialogData } from '../user-form-dialog/user-form-dialog.component';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html'
})
export class UserListComponent implements OnInit {
  readonly displayedColumns = ['name', 'userName', 'email', 'roles', 'status', 'actions'];

  users: User[] = [];
  isLoading = false;

  constructor(
    private userService: UserService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.isLoading = true;
    this.userService.getUsers()
      .pipe(finalize(() => this.isLoading = false))
      .subscribe(users => this.users = users);
  }

  openUserDialog(user?: User): void {
    this.dialog.open<UserFormDialogComponent, UserFormDialogData, boolean>(UserFormDialogComponent, {
      width: '600px',
      data: { user }
    }).afterClosed().subscribe(saved => {
      if (saved) {
        this.loadUsers();
      }
    });
  }

  unlockUser(user: User): void {
    this.userService.unlockUser(user.id).subscribe(() => this.loadUsers());
  }

  deleteUser(user: User): void {
    this.dialog.open<ConfirmDialogComponent, ConfirmDialogData, boolean>(ConfirmDialogComponent, {
      data: {
        title: 'Usuń użytkownika',
        message: `Czy na pewno chcesz usunąć konto ${user.userName}?`
      }
    }).afterClosed().pipe(
      filter(confirmed => !!confirmed),
      switchMap(() => this.userService.deleteUser(user.id))
    ).subscribe(() => this.loadUsers());
  }
}
