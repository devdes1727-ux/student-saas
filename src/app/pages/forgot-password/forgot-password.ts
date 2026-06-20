import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../../core/services/api.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './forgot-password.html',
  styleUrls: ['../login/login.css']
})
export class ForgotPassword {
  email = '';
  loading = false;
  error = '';
  success = '';

  constructor(private api: ApiService, private router: Router) {}

  submit() {
    if (!this.email) { this.error = 'Please enter your email address.'; return; }
    this.loading = true; this.error = '';

    this.api.forgotPassword(this.email).subscribe({
      next: () => {
        this.success = 'If that email exists, a password reset link has been sent.';
        this.loading = false;
      },
      error: () => {
        this.success = 'If that email exists, a password reset link has been sent.';
        this.loading = false;
      }
    });
  }
}
