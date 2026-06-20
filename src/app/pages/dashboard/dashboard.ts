import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../core/services/api.service';
import { SessionService } from '../../core/services/session.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})
export class Dashboard implements OnInit {

  stats: any = {};
  recentPayments: any[] = [];
  overdueStudents: any[] = [];
  loading = true;

  constructor(
    private api: ApiService,
    public session: SessionService,
    private cdr: ChangeDetectorRef,
    private toast: ToastService
  ) { }

  ngOnInit() { this.load(); }

  load() {
    this.loading = true;
    this.api.getDashboard().subscribe({
      next: (data: any) => {
        this.stats = data;
        this.recentPayments = data.recentPayments || [];
        this.overdueStudents = data.overdueStudents || [];
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toast.error('Failed to load dashboard');
        this.loading = false;
      }
    });
  }

  get role() { return this.session.getRole(); }
  get isAdmin() {
    const r = this.role;
    return r === 'Admin' || r === 'InstituteAdmin' || r === 'SuperAdmin';
  }

  formatCurrency(v: number) {
    return '₹' + (v || 0).toLocaleString('en-IN');
  }
}