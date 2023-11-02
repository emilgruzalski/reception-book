import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { finalize } from 'rxjs';

import { Room } from 'src/app/core/models/room.model';
import { RoomService } from 'src/app/core/services/room.service';
import { RoomFormDialogComponent, RoomFormDialogData } from '../room-form-dialog/room-form-dialog.component';

@Component({
  selector: 'app-room-list',
  templateUrl: './room-list.component.html',
  styleUrls: ['./room-list.component.scss']
})
export class RoomListComponent implements OnInit {
  readonly displayedColumns = ['number', 'floor', 'type', 'capacity', 'pricePerNight', 'status', 'actions'];

  rooms: Room[] = [];
  isLoading = false;

  constructor(
    private roomService: RoomService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadRooms();
  }

  loadRooms(): void {
    this.isLoading = true;
    this.roomService.getRooms()
      .pipe(finalize(() => this.isLoading = false))
      .subscribe(rooms => this.rooms = rooms);
  }

  openRoomDialog(room?: Room): void {
    this.dialog.open<RoomFormDialogComponent, RoomFormDialogData, boolean>(RoomFormDialogComponent, {
      width: '600px',
      data: { room }
    }).afterClosed().subscribe(saved => {
      if (saved) {
        this.loadRooms();
      }
    });
  }
}
