import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export interface DashboardStats {
  totalInstitutes: number;
  totalStudents: number;
  totalStaff: number;
  totalCourses: number;
  monthlyRevenue: number;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  private apiUrl = 'http://localhost:5164/api/Dashboard';

  constructor(private http: HttpClient) {}

  getDashboard() {
    return this.http.get<DashboardStats>(this.apiUrl);
  }
}