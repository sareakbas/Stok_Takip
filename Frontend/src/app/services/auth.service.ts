import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginRequest, LoginResponse, RegisterRequest,RegisterResponse} from '../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = 'http://localhost:5237/api/auth';

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<LoginResponse> {
    const request: LoginRequest = {
      email,
      password
    };

    return this.http.post<LoginResponse>(
      `${this.apiUrl}/login`,
      request
    );
  }

  register(
    fullName: string,
    email: string,
    password: string
  ): Observable<RegisterResponse> {
    const request: RegisterRequest = {
      fullName,
      email,
      password
    };

    return this.http.post<RegisterResponse>(
      `${this.apiUrl}/register`,
      request
    );
  }

  saveToken(token: string): void {
    localStorage.setItem('token', token);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }
}