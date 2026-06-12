import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardService } from '../../core/services/dashboard.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})
export class Dashboard implements OnInit {

  stats = {
    totalInstitutes: 0,
    totalStudents: 0,
    totalStaff: 0,
    totalCourses: 0,
    monthlyRevenue: 0
  };

  recentPayments: any[] = [];

  dueStudents: string[] = [];

  constructor(
    private dashboardService: DashboardService
  ) { }

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard() {

    this.dashboardService.getDashboard()
      .subscribe({
        next: (data) => {

          this.stats = data;

        },
        error: (err) => {
          console.error(err);
        }
      });

  }
}