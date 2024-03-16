import { Component, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import { Sort } from '@angular/material/sort';
import { debounceTime, filter, finalize, switchMap } from 'rxjs';

import { SEARCH_DEBOUNCE_MS } from 'src/app/core/constants';
import { ADMINISTRATOR_ONLY } from 'src/app/core/models/auth.model';
import { DEFAULT_PAGE_SIZE, PAGE_SIZE_OPTIONS } from 'src/app/core/models/paged-result.model';
import { Room, ROOM_STATUSES, ROOM_TYPES, RoomQuery, RoomStatus, RoomType } from 'src/app/core/models/room.model';
import { RoomService } from 'src/app/core/services/room.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { ConfirmDialogComponent, ConfirmDialogData } from 'src/app/shared/components/confirm-dialog/confirm-dialog.component';
import { RoomFormDialogComponent, RoomFormDialogData } from '../room-form-dialog/room-form-dialog.component';

const DEFAULT_ORDER_BY = 'number';

@Component({
  selector: 'app-room-list',
  templateUrl: './room-list.component.html',
  styleUrls: ['./room-list.component.scss']
})
export class RoomListComponent implements OnInit {
  readonly administratorOnly = ADMINISTRATOR_ONLY;
  readonly displayedColumns = ['number', 'floor', 'type', 'capacity', 'pricePerNight', 'status', 'actions'];
  readonly pageSizeOptions = PAGE_SIZE_OPTIONS;
  readonly roomTypes = ROOM_TYPES;
  readonly roomStatuses = ROOM_STATUSES;

  readonly filtersForm = this.fb.group({
    searchTerm: [''],
    type: [null as RoomType | null],
    status: [null as RoomStatus | null]
  });

  rooms: Room[] = [];
  totalCount = 0;
  query: RoomQuery = { pageNumber: 1, pageSize: DEFAULT_PAGE_SIZE, orderBy: DEFAULT_ORDER_BY };
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private roomService: RoomService,
    private dialog: MatDialog,
    private notificationService: NotificationService
  ) {
    this.filtersForm.valueChanges.pipe(
      debounceTime(SEARCH_DEBOUNCE_MS),
      takeUntilDestroyed()
    ).subscribe(filters => {
      // Filters change the number of results, so always go back to the first page.
      this.query = {
        ...this.query,
        pageNumber: 1,
        searchTerm: filters.searchTerm ?? undefined,
        type: filters.type ?? undefined,
        status: filters.status ?? undefined
      };
      this.loadRooms();
    });
  }

  ngOnInit(): void {
    this.loadRooms();
  }

  loadRooms(): void {
    this.isLoading = true;
    this.roomService.getRooms(this.query)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe(result => {
        this.rooms = result.items;
        this.totalCount = result.pagination.totalCount;
      });
  }

  onPageChange(event: PageEvent): void {
    this.query = { ...this.query, pageNumber: event.pageIndex + 1, pageSize: event.pageSize };
    this.loadRooms();
  }

  onSortChange(sort: Sort): void {
    const orderBy = sort.direction ? `${sort.active} ${sort.direction}` : DEFAULT_ORDER_BY;
    this.query = { ...this.query, orderBy };
    this.loadRooms();
  }

  changeStatus(room: Room, status: RoomStatus): void {
    this.roomService.updateRoomStatus(room.id, status).subscribe(() => {
      this.notificationService.success(`Zmieniono status pokoju ${room.number}.`);
      this.loadRooms();
    });
  }

  openRoomDialog(room?: Room): void {
    this.dialog.open<RoomFormDialogComponent, RoomFormDialogData, boolean>(RoomFormDialogComponent, {
      width: '600px',
      data: { room }
    }).afterClosed().subscribe(saved => {
      if (saved) {
        this.notificationService.success('Zapisano dane pokoju.');
        this.loadRooms();
      }
    });
  }

  deleteRoom(room: Room): void {
    this.dialog.open<ConfirmDialogComponent, ConfirmDialogData, boolean>(ConfirmDialogComponent, {
      data: {
        title: 'Usuń pokój',
        message: `Czy na pewno chcesz usunąć pokój ${room.number}?`
      }
    }).afterClosed().pipe(
      filter(confirmed => !!confirmed),
      switchMap(() => this.roomService.deleteRoom(room.id))
    ).subscribe(() => {
      this.notificationService.success(`Usunięto pokój ${room.number}.`);
      this.loadRooms();
    });
  }
}
