import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ApiService } from '../../core/services/api.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reset-password.html',
  styleUrls: ['../login/login.css']
})
export class ResetPassword implements OnInit {
  token = '';
  email = '';
  password = '';
  confirmPassword = '';
  loading = false;
  error = '';
  success = '';

  constructor(private api: ApiService, private router: Router, private route: ActivatedRoute) {}

  ngOnInit() {
    this.token = this.route.snapshot.queryParamMap.get('token') || '';
    this.email = this.route.snapshot.queryParamMap.get('email') || '';
  }

  submit() {
    if (!this.password || !this.confirmPassword) {
      this.error = 'Please fill in all fields.'; return;
    }
    if (this.password !== this.confirmPassword) {
      this.error = 'Passwords do not match.'; return;
    }
    if (this.password.length < 6) {
      this.error = 'Password must be at least 6 characters.'; return;
    }
    this.loading = true; this.error = '';

    this.api.resetPassword({ token: this.token, email: this.email, newPassword: this.password }).subscribe({
      next: () => {
        this.success = 'Password reset successfully! Redirecting to login...';
        this.loading = false;
        setTimeout(() => this.router.navigate(['/login']), 2500);
      },
      error: (err) => {
        this.error = err?.error?.message || 'Reset link is invalid or has expired.';
        this.loading = false;
      }
    });
  }
}
