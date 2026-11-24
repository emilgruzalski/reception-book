import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from 'src/environments/environment';
import { Dashboard } from '../models/dashboard.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly url = `${environment.apiUrl}/dashboard`;

  constructor(private http: HttpClient) { }

  getDashboard(): Observable<Dashboard> {
    return this.http.get<Dashboard>(this.url);
  }
}
