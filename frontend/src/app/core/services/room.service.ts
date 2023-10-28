import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from 'src/environments/environment';
import { Room, RoomForManipulation } from '../models/room.model';

@Injectable({
  providedIn: 'root'
})
export class RoomService {
  private readonly url = `${environment.apiUrl}/rooms`;

  constructor(private http: HttpClient) { }

  getRooms(): Observable<Room[]> {
    return this.http.get<Room[]>(this.url);
  }

  getRoom(id: string): Observable<Room> {
    return this.http.get<Room>(`${this.url}/${id}`);
  }

  createRoom(room: RoomForManipulation): Observable<Room> {
    return this.http.post<Room>(this.url, room);
  }

  updateRoom(id: string, room: RoomForManipulation): Observable<void> {
    return this.http.put<void>(`${this.url}/${id}`, room);
  }

  deleteRoom(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
