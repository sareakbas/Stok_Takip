import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Card } from 'primeng/card';
import { InputText } from 'primeng/inputtext';
import { Password } from 'primeng/password';
import { Button } from 'primeng/button';
import { FloatLabel } from 'primeng/floatlabel';

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
    FloatLabel
  ],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {
    username = '';
    password = '';

    login() {
        console.log('Giriş isteği atılıyor...', this.username, this.password);
    }
}