import { Inject, inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_CONFIG, ApiConfig } from '../../../shared/api.config';
import { Observable } from 'rxjs';
import { User } from '../user.model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly http = inject(HttpClient);

  private readonly ENDPOINT: string;

  constructor(@Inject(API_CONFIG) private readonly apiConfig: ApiConfig) {
    this.ENDPOINT = `${this.apiConfig.baseUrl}/${this.apiConfig.prefix}`;
  }

  getUserInfo(): Observable<User> {
    // TODO handle errors
    return this.http.get<User>(`${this.ENDPOINT}/manage/info`, {
      withCredentials: true,
    });
  }
}
