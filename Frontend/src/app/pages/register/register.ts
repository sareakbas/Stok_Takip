import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { InputText } from 'primeng/inputtext';
import { Password } from 'primeng/password';
import { Button } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

import { AuthService } from '../../services/auth.service';
import { RegisterRequest } from '../../models/register-request.model';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    InputText,
    Password,
    Button,
    ToastModule
  ],
  providers: [MessageService],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  registerRequest = new RegisterRequest();
  confirmPassword = '';

  constructor(
    private authService: AuthService,
    private messageService: MessageService
  ) {}

  register(): void {
    if (
      !this.registerRequest.fullName.trim() ||
      !this.registerRequest.email.trim() ||
      !this.registerRequest.password ||
      !this.confirmPassword
    ) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Eksik Bilgi',
        detail: 'Lütfen tüm alanları doldurun.',
        life: 3000
      });

      return;
    }

    if (this.registerRequest.password !== this.confirmPassword) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Şifre Hatası',
        detail: 'Şifreler birbiriyle eşleşmiyor.',
        life: 3000
      });

      return;
    }

    this.registerRequest.fullName =
      this.registerRequest.fullName.trim();

    this.registerRequest.email =
      this.registerRequest.email.trim();

    this.authService
      .register(this.registerRequest)
      .subscribe({
        next: (response) => {
          this.messageService.add({
            severity: 'success',
            summary: 'Kayıt Başarılı',
            detail:
              response.message ||
              'Kullanıcı başarıyla kaydedildi.',
            life: 3000
          });

          this.clearForm();
        },

        error: (error) => {
          const errorMessage =
            error.error?.message ||
            'Kayıt işlemi gerçekleştirilemedi.';

          this.messageService.add({
            severity: 'error',
            summary: 'Kayıt Başarısız',
            detail: errorMessage,
            life: 4000
          });
        }
      });
  }

  private clearForm(): void {
    this.registerRequest = new RegisterRequest();
    this.confirmPassword = '';
  }
}