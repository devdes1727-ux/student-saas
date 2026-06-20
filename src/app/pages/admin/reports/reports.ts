import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-admin-reports',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reports.html',
  styleUrls: ['./reports.css']
})
export class AdminReports implements OnInit {
  stats: any = {};
  loading = true;

  constructor(private api: ApiService, private toast: ToastService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.loadReportsData();
  }

  loadReportsData() {
    this.loading = true;
    this.api.getDashboard().subscribe({
      next: (data) => {
        this.stats = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toast.error('Failed to load reports summary');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  exportPaymentsReport() {
    this.api.getPayments().subscribe({
      next: (payments) => {
        if (payments.length === 0) {
          this.toast.error('No payments data available to export.');
          return;
        }
        
        let csvContent = "data:text/csv;charset=utf-8,";
        csvContent += "Payment ID,Student Name,Course,Paid Amount,Balance Amount,Payment Date,Payment Method,Status\n";
        
        payments.forEach((p: any) => {
          const row = [
            p.id,
            `"${p.studentName}"`,
            `"${p.courseName}"`,
            p.paidAmount,
            p.balanceAmount,
            new Date(p.paymentDate).toLocaleDateString(),
            p.paymentMethod,
            p.status
          ].join(",");
          csvContent += row + "\n";
        });
        
        const encodedUri = encodeURI(csvContent);
        const link = document.createElement("a");
        link.setAttribute("href", encodedUri);
        link.setAttribute("download", `billing_report_${new Date().toISOString().split('T')[0]}.csv`);
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        
        this.toast.success('Payments list exported to CSV successfully');
      },
      error: () => this.toast.error('Failed to fetch payments data for export')
    });
  }

  formatCurrency(value: number): string {
    return '₹' + (value || 0).toLocaleString('en-IN');
  }
}
