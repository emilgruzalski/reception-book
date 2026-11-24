import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from 'src/environments/environment';
import { User, UserForRegistration, UserForUpdate } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly url = `${environment.apiUrl}/users`;
  private readonly registrationUrl = `${environment.apiUrl}/authentication`;

  constructor(private http: HttpClient) { }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.url);
  }

  registerUser(user: UserForRegistration): Observable<void> {
    return this.http.post<void>(this.registrationUrl, user);
  }

  updateUser(id: string, user: UserForUpdate): Observable<void> {
    return this.http.put<void>(`${this.url}/${id}`, user);
  }

  unlockUser(id: string): Observable<void> {
    return this.http.post<void>(`${this.url}/${id}/unlock`, null);
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
