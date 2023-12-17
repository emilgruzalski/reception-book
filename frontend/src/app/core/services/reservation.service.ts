import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';

import { environment } from 'src/environments/environment';
import { PagedResult } from '../models/paged-result.model';
import { Reservation, ReservationForManipulation, ReservationQuery } from '../models/reservation.model';
import { toHttpParams, toPagedResult } from '../utils/http.utils';

@Injectable({
  providedIn: 'root'
})
export class ReservationService {
  private readonly url = `${environment.apiUrl}/reservations`;

  constructor(private http: HttpClient) { }

  getReservations(query: ReservationQuery): Observable<PagedResult<Reservation>> {
    return this.http.get<Reservation[]>(this.url, { params: toHttpParams(query), observe: 'response' })
      .pipe(map(response => toPagedResult(response)));
  }

  getReservation(id: string): Observable<Reservation> {
    return this.http.get<Reservation>(`${this.url}/${id}`);
  }

  createReservation(reservation: ReservationForManipulation): Observable<Reservation> {
    return this.http.post<Reservation>(this.url, reservation);
  }

  updateReservation(id: string, reservation: ReservationForManipulation): Observable<void> {
    return this.http.put<void>(`${this.url}/${id}`, reservation);
  }

  checkIn(id: string): Observable<void> {
    return this.http.post<void>(`${this.url}/${id}/check-in`, null);
  }

  checkOut(id: string): Observable<void> {
    return this.http.post<void>(`${this.url}/${id}/check-out`, null);
  }

  cancel(id: string): Observable<void> {
    return this.http.post<void>(`${this.url}/${id}/cancel`, null);
  }

  deleteReservation(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
