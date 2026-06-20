import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})
export class AdminDashboard implements OnInit {
  stats: any = {};
  recentActivities: any[] = [];
  upcomingEvents: any[] = [];
  pendingEnquiries: any[] = [];
  notifications: any[] = [];
  loading = true;

  constructor(
    private api: ApiService,
    private toast: ToastService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadDashboardData();
  }

  loadDashboardData() {
    this.loading = true;
    this.api.getDashboard().subscribe({
      next: (data) => {
        this.stats = data;
        this.recentActivities = data.recentActivities || [];
        this.upcomingEvents = data.upcomingEvents || [];
        this.notifications = data.notifications || [];
        this.loadPendingEnquiries();
      },
      error: (err) => {
        this.toast.error('Failed to load dashboard data');
        this.loading = false;
      }
    });
  }

  loadPendingEnquiries() {
    this.api.getEnquiries().subscribe({
      next: (data) => {
        this.pendingEnquiries = data.filter((e: any) => e.status === 'Pending').slice(0, 5);
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  approveEnquiry(id: number) {
    this.api.updateEnquiryStatus(id, 'Approved').subscribe({
      next: () => {
        this.toast.success('Enquiry approved');
        this.loadPendingEnquiries();
      },
      error: () => this.toast.error('Failed to update enquiry status')
    });
  }

  rejectEnquiry(id: number) {
    this.api.updateEnquiryStatus(id, 'Rejected').subscribe({
      next: () => {
        this.toast.success('Enquiry marked as rejected');
        this.loadPendingEnquiries();
      },
      error: () => this.toast.error('Failed to update enquiry status')
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

  clearAllNotifications() {
    this.api.markAllNotificationsAsRead().subscribe({
      next: () => {
        this.notifications = [];
        this.toast.success('All notifications cleared');
      }
    });
  }

  formatCurrency(value: number): string {
    return '₹' + (value || 0).toLocaleString('en-IN');
  }

  getMaxGrowthCount(): number {
    if (!this.stats.studentGrowth || this.stats.studentGrowth.length === 0) return 10;
    return Math.max(...this.stats.studentGrowth.map((g: any) => g.count), 5);
  }

  getMaxRevenueTrend(): number {
    if (!this.stats.revenueTrend || this.stats.revenueTrend.length === 0) return 1000;
    return Math.max(...this.stats.revenueTrend.map((r: any) => r.revenue), 1000);
  }
}
