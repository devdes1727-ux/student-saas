import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class AuthService {

    private apiUrl = `${environment.apiBase}/auth`;

    constructor(private http: HttpClient) { }

    login(data: any) {
        return this.http.post(`${this.apiUrl}/login`, data);
    }

    register(data: any) {
        return this.http.post(`${this.apiUrl}/register`, data);
    }

    saveSession(res: any) {
        localStorage.setItem('token', res.token);
        localStorage.setItem('user', JSON.stringify(res.user));
    }

    logout() {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
    }

    getToken(): string | null {
        if (typeof window === 'undefined') return null;
        return localStorage.getItem('token');
    }

    getCurrentUser(): any {
        if (typeof window === 'undefined') return null;
        const user = localStorage.getItem('user');
        return user ? JSON.parse(user) : null;
    }

    isLoggedIn(): boolean {
        return !!this.getToken();
    }
}