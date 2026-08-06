import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { Card } from 'primeng/card';
import { InputText } from 'primeng/inputtext';
import { Password } from 'primeng/password';
import { Button } from 'primeng/button';
import { FloatLabel } from 'primeng/floatlabel';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

import { AuthService } from '../../services/auth.service';
import { LoginRequest } from '../../models/login-request.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    Card,
    InputText,
    Password,
    Button,
    FloatLabel,
    RouterLink,
    ToastModule
  ],
  providers: [MessageService],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {
  loginRequest = new LoginRequest();

  constructor(
    private authService: AuthService,
    private router: Router,
    private messageService: MessageService
  ) {}

  login(): void {
    if (
      !this.loginRequest.email.trim() ||
      !this.loginRequest.password
    ) {
      
      this.messageService.add({
        severity: 'warn',
        summary: 'Eksik Bilgi',
        detail: 'Lütfen e-posta ve şifre alanlarını doldurun.',
        life: 3000
      });

      return;
    }

    this.loginRequest.email = this.loginRequest.email.trim();

    this.authService
      .login(this.loginRequest)
      .subscribe({
        next: (response) => {
          this.authService.saveToken(response.data);

          this.messageService.add({
            severity: 'success',
            summary: 'Giriş Başarılı',
            detail: response.message || 'Sisteme başarıyla giriş yaptınız.',
            life: 2000
          });

          setTimeout(() => {
            this.router.navigate(['/dashboard']);
          }, 1000);
        },

        error: (error) => {
          const errorMessage =
            error.error?.message ||
            'Giriş işlemi gerçekleştirilemedi.';

          this.messageService.add({
            severity: 'error',
            summary: 'Giriş Başarısız',
            detail: errorMessage,
            life: 4000
          });
        }
      });
  }
}