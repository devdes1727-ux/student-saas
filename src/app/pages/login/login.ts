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

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  login() {

    if (!this.email || !this.password) {
      this.errorMessage = 'Enter Email and Password';
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    this.authService.login({
      email: this.email,
      password: this.password
    }).subscribe({
      next: (res: any) => {

        // ✅ CENTRALIZED SESSION SAVE
        this.authService.saveSession(res);

        const role = res?.user?.role;

        // ✅ CLEAN ROLE ROUTING
        switch (role) {
          case 'SuperAdmin':
            this.router.navigate(['/dashboard']);
            break;

          case 'InstituteAdmin':
            this.router.navigate(['/dashboard']);
            break;

          default:
            this.router.navigate(['/dashboard']);
            break;
        }

        this.loading = false;
      },

      error: (err) => {
        this.errorMessage = 'Invalid Email or Password';
        this.loading = false;
      }
    });
  }
}