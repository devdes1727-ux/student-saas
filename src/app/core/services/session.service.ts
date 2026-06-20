import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class SessionService {

  private platformId = inject(PLATFORM_ID);

  getUser(): any {

    if (!isPlatformBrowser(this.platformId)) {
      return null;
    }

    return JSON.parse(localStorage.getItem('user') || 'null');
  }

  getRole(): string {
    const user = this.getUser();
    return user?.role || '';
  }

  isSuperAdmin(): boolean {
    return this.getRole() === 'SuperAdmin';
  }

  isInstituteAdmin(): boolean {
    return this.getRole() === 'InstituteAdmin';
  }

  isLoggedIn(): boolean {

    if (!isPlatformBrowser(this.platformId)) {
      return false;
    }

    return !!localStorage.getItem('user');
  }

  logout() {

    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem('user');
      localStorage.removeItem('token');
    }

  }
}