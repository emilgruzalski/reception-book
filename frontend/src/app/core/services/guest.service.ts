import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';

import { environment } from 'src/environments/environment';
import { Guest, GuestForManipulation, GuestQuery } from '../models/guest.model';
import { PagedResult } from '../models/paged-result.model';
import { toHttpParams, toPagedResult } from '../utils/http.utils';

@Injectable({
  providedIn: 'root'
})
export class GuestService {
  private readonly url = `${environment.apiUrl}/guests`;

  constructor(private http: HttpClient) { }

  getGuests(query: GuestQuery): Observable<PagedResult<Guest>> {
    return this.http.get<Guest[]>(this.url, { params: toHttpParams(query), observe: 'response' })
      .pipe(map(response => toPagedResult(response)));
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
