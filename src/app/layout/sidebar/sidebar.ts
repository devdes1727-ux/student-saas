import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { PLATFORM_ID, inject } from '@angular/core';
import { SessionService } from '../../core/services/session.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterModule, CommonModule],
  templateUrl: './sidebar.html',
  styleUrls: ['./sidebar.css']
})
export class Sidebar implements OnInit {

  private platformId = inject(PLATFORM_ID);
  user: any = null;

  constructor(private router: Router, public session: SessionService) {}

  ngOnInit() {
    if (isPlatformBrowser(this.platformId)) {
      this.user = this.session.getUser();
    }
  }

  get role(): string { return this.user?.role || ''; }

  get prefix(): string {
    if (this.isAdmin) return '/admin';
    if (this.role === 'Teacher') return '/teacher';
    return '';
  }

  get isAdmin(): boolean {
    return this.role === 'Admin' || this.role === 'InstituteAdmin' || this.role === 'SuperAdmin';
  }

  get isAdminOrTeacher(): boolean {
    return this.isAdmin || this.role === 'Teacher';
  }

  get isStudent(): boolean {
    return this.role === 'Student';
  }

  logout() {
    this.session.logout();
    this.router.navigate(['/login']);
  }
}