import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../../core/services/api.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './register.html',
  styleUrls: ['../login/login.css']
})
export class Register {
  name = '';
  email = '';
  password = '';
  role = 'Student';
  loading = false;
  error = '';
  success = '';

  constructor(private api: ApiService, private router: Router) {}

  register() {
    if (!this.name || !this.email || !this.password) {
      this.error = 'All fields are required.';
      return;
    }
    if (this.password.length < 6) {
      this.error = 'Password must be at least 6 characters.';
      return;
    }
    this.loading = true;
    this.error = '';

    this.api.register({ name: this.name, email: this.email, password: this.password, role: this.role })
      .subscribe({
        next: () => {
          this.success = 'Account created! You can now sign in.';
          this.loading = false;
          setTimeout(() => this.router.navigate(['/login']), 2000);
        },
        error: (err) => {
          this.error = err?.error?.message || 'Registration failed. Please try again.';
          this.loading = false;
        }
      });
  }
}
