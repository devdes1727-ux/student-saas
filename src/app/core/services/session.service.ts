import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SessionService {

  getUser(): any {
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

  logout() {
    localStorage.removeItem('user');
    localStorage.removeItem('token');
  }
}