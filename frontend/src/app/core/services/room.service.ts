import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';

import { environment } from 'src/environments/environment';
import { Room, RoomForManipulation, RoomQuery, RoomStatus } from '../models/room.model';
import { PagedResult } from '../models/paged-result.model';
import { toHttpParams, toPagedResult } from '../utils/http.utils';

@Injectable({
  providedIn: 'root'
})
export class RoomService {
  private readonly url = `${environment.apiUrl}/rooms`;

  constructor(private http: HttpClient) { }

  getRooms(query: RoomQuery): Observable<PagedResult<Room>> {
    return this.http.get<Room[]>(this.url, { params: toHttpParams(query), observe: 'response' })
      .pipe(map(response => toPagedResult(response)));
  }

  getAvailableRooms(checkInDate: string, checkOutDate: string, numberOfGuests: number): Observable<Room[]> {
    const params = toHttpParams({ checkInDate, checkOutDate, numberOfGuests });
    return this.http.get<Room[]>(`${this.url}/available`, { params });
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

  updateRoomStatus(id: string, status: RoomStatus): Observable<void> {
    return this.http.put<void>(`${this.url}/${id}/status`, { status });
  }

  deleteRoom(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
