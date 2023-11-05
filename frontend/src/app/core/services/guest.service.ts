import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from 'src/environments/environment';
import { Guest, GuestForManipulation } from '../models/guest.model';

@Injectable({
  providedIn: 'root'
})
export class GuestService {
  private readonly url = `${environment.apiUrl}/guests`;

  constructor(private http: HttpClient) { }

  getGuests(): Observable<Guest[]> {
    return this.http.get<Guest[]>(this.url);
  }

  getGuest(id: string): Observable<Guest> {
    return this.http.get<Guest>(`${this.url}/${id}`);
  }

  createGuest(guest: GuestForManipulation): Observable<Guest> {
    return this.http.post<Guest>(this.url, guest);
  }

  updateGuest(id: string, guest: GuestForManipulation): Observable<void> {
    return this.http.put<void>(`${this.url}/${id}`, guest);
  }

  deleteGuest(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
