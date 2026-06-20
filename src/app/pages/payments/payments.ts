import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-payments',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './payments.html',
  styleUrls: ['./payments.css']
})
export class Payments implements OnInit {

  payments: any[] = [];
  filtered: any[] = [];
  pending: any[] = [];
  courses: any[] = [];
  students: any[] = [];
  loading = false;
  activeTab = 'all'; // 'all' | 'pending'
  searchTerm = '';
  filterMonth = '';

  page = 1;
  pageSize = 10;

  showModal = false;
  isEdit = false;
  saving = false;
  form: any = this.emptyForm();

  constructor(
    private api: ApiService,
    private toast: ToastService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.load();
    this.api.getCourses().subscribe({ next: (d: any) => this.courses = Array.isArray(d) ? d : [] });
    this.api.getStudents().subscribe({ next: (d: any) => this.students = Array.isArray(d) ? d : (d.items || []) });
  }

  emptyForm() {
    return { id: null, studentId: '', courseId: '', amount: '', paidOn: new Date().toISOString().substring(0, 10), notes: '', status: 'Paid', month: '', year: new Date().getFullYear() };
  }

  load() {
    this.loading = true;
    this.api.getPayments().subscribe({
      next: (d: any) => {
        this.payments = Array.isArray(d) ? d : (d.items || []);
        this.applyFilter();
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => { this.toast.error('Failed to load payments'); this.loading = false; }
    });
    this.api.getPendingPayments().subscribe({
      next: (d: any) => this.pending = Array.isArray(d) ? d : [],
      error: () => {
        this.toast.error('Failed to load pending payments');
        this.loading = false;
      }
    });
  }

  applyFilter() {
    const term = this.searchTerm.toLowerCase();
    this.filtered = this.payments.filter(p =>
      (!term || p.studentName?.toLowerCase().includes(term) || p.courseName?.toLowerCase().includes(term))
    );
    this.page = 1;
  }

  get paginated() {
    const start = (this.page - 1) * this.pageSize;
    return this.filtered.slice(start, start + this.pageSize);
  }
  get totalPages() { return Math.ceil(this.filtered.length / this.pageSize); }

  openAdd() { this.form = this.emptyForm(); this.isEdit = false; this.showModal = true; }
  openEdit(p: any) { this.form = { ...p, paidOn: p.paidOn?.substring(0, 10) }; this.isEdit = true; this.showModal = true; }
  closeModal() { this.showModal = false; }

  save() {
    if (!this.form.studentId || !this.form.amount) { this.toast.warn('Student and amount are required'); return; }
    this.saving = true;
    const call = this.isEdit ? this.api.updatePayment(this.form.id, this.form) : this.api.createPayment(this.form);
    call.subscribe({
      next: () => { this.toast.success(this.isEdit ? 'Payment updated!' : 'Payment recorded!'); this.showModal = false; this.saving = false; this.load(); },
      error: (e) => { this.toast.error(e?.error?.message || 'Save failed'); this.saving = false; }
    });
  }

  delete(p: any) {
    if (!confirm(`Delete this payment record?`)) return;
    this.api.deletePayment(p.id).subscribe({
      next: () => { this.toast.success('Deleted'); this.load(); },
      error: () => this.toast.error('Delete failed')
    });
  }

  statusClass(s: string) {
    return s === 'Paid' ? 'badge-success' : s === 'Partial' ? 'badge-warning' : 'badge-danger';
  }

  get totalRevenue() { return this.payments.reduce((s, p) => s + (p.amount || 0), 0); }
}
