import { Component, OnInit } from '@angular/core';
import { finalize } from 'rxjs';

import { Room } from 'src/app/core/models/room.model';
import { RoomService } from 'src/app/core/services/room.service';

@Component({
  selector: 'app-room-list',
  templateUrl: './room-list.component.html',
  styleUrls: ['./room-list.component.scss']
})
export class RoomListComponent implements OnInit {
  readonly displayedColumns = ['number', 'floor', 'type', 'capacity', 'pricePerNight', 'status'];

  rooms: Room[] = [];
  isLoading = false;

  constructor(private roomService: RoomService) { }

  ngOnInit(): void {
    this.loadRooms();
  }

  loadRooms(): void {
    this.isLoading = true;
    this.roomService.getRooms()
      .pipe(finalize(() => this.isLoading = false))
      .subscribe(rooms => this.rooms = rooms);
  }
}
