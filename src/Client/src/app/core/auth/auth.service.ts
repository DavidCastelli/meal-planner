import { Inject, inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_CONFIG, ApiConfig } from '../../shared/api-config';
import { Observable } from 'rxjs';
import { UserModel } from './user.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);

  private readonly ENDPOINT: string;

  constructor(@Inject(API_CONFIG) private readonly apiConfig: ApiConfig) {
    this.ENDPOINT = `${apiConfig.baseUrl}/${apiConfig.prefix}`;
  }

  register(credentials: {
    email: string;
    password: string;
  }): Observable<UserModel> {
    return this.http.post<UserModel>(`${this.ENDPOINT}/register`, credentials);
  }
}
