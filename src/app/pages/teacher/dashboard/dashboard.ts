import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-teacher-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})
export class TeacherDashboard implements OnInit {
  stats: any = {};
  batches: any[] = [];
  recentActivities: any[] = [];
  notifications: any[] = [];
  loading = true;

  constructor(private api: ApiService, private toast: ToastService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.loadDashboard();
  }

  loadDashboard() {
    this.loading = true;
    this.api.getDashboard().subscribe({
      next: (data) => {
        this.stats = data;
        this.recentActivities = data.recentActivities || [];
        this.notifications = data.notifications || [];
        this.loadTeacherBatches();
      },
      error: () => {
        this.toast.error('Failed to load dashboard metrics');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadTeacherBatches() {
    this.api.getBatches().subscribe({
      next: (data) => {
        this.batches = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  dismissNotification(id: number) {
    this.api.markNotificationAsRead(id).subscribe({
      next: () => {
        this.notifications = this.notifications.filter(n => n.id !== id);
        this.toast.success('Notification cleared');
      }
    });
  }
}
