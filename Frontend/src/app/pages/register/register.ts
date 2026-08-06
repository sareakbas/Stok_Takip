import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { InputText } from 'primeng/inputtext';
import { Password } from 'primeng/password';
import { Button } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

import { AuthService } from '../../services/auth.service';

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
  fullName = '';
  email = '';
  password = '';
  confirmPassword = '';

  constructor(
    private authService: AuthService,
    private messageService: MessageService
  ) {}

  register(): void {
    if (
      !this.fullName.trim() ||
      !this.email.trim() ||
      !this.password ||
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

    if (this.password !== this.confirmPassword) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Şifre Hatası',
        detail: 'Şifreler birbiriyle eşleşmiyor.',
        life: 3000
      });

      return;
    }

    this.authService
      .register(
        this.fullName.trim(),
        this.email.trim(),
        this.password
      )
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
    this.fullName = '';
    this.email = '';
    this.password = '';
    this.confirmPassword = '';
  }
}