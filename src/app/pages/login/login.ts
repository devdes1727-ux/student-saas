import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrls: ['./login.css'],
})
export class Login {
  email = '';
  password = '';
  loading = false;
  errorMessage = '';

  constructor(private authService: AuthService, private router: Router) {}

  login() {
    if (!this.email || !this.password) {
      this.errorMessage = 'Please enter your email and password.';
      return;
    }
    this.loading = true;
    this.errorMessage = '';

    this.authService.login({ email: this.email, password: this.password }).subscribe({
      next: (res: any) => {
        this.authService.saveSession(res);
        const role = res.user?.role;
        if (role === 'Admin' || role === 'InstituteAdmin' || role === 'SuperAdmin') {
          this.router.navigate(['/admin/dashboard']);
        } else if (role === 'Teacher') {
          this.router.navigate(['/teacher/dashboard']);
        } else {
          this.router.navigate(['/']);
        }
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Invalid email or password. Please try again.';
        this.loading = false;
      }
    });
  }

  goRegister() { this.router.navigate(['/register']); }
  goForgot() { this.router.navigate(['/forgot-password']); }
}