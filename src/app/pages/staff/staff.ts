import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-staff',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './staff.html',
  styleUrls: ['./staff.css']
})
export class Staff implements OnInit {

  staffList: any[] = [];
  filtered: any[] = [];
  loading = false;
  searchTerm = '';
  filterStatus = '';

  page = 1;
  pageSize = 10;

  showModal = false;
  isEdit = false;
  saving = false;
  form: any = this.emptyForm();

  constructor(private api: ApiService,
    private toast: ToastService,
    private cdr: ChangeDetectorRef) { }

  ngOnInit() { this.load(); }

  emptyForm() {
    return { id: null, name: '', email: '', phone: '', role: 'Teacher', specialization: '', status: 'Active', salary: '' };
  }

  load() {
    this.loading = true;
    this.api.getStaff().subscribe({
      next: (d: any) => {
        this.staffList = Array.isArray(d) ? d : (d.items || []);
        this.applyFilter();
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => { this.toast.error('Failed to load staff'); this.loading = false; }
    });
  }

  applyFilter() {
    const term = this.searchTerm.toLowerCase();
    this.filtered = this.staffList.filter(s =>
      (!term || s.name?.toLowerCase().includes(term) || s.email?.toLowerCase().includes(term)) &&
      (!this.filterStatus || s.status === this.filterStatus)
    );
    this.page = 1;
  }

  get paginated() {
    const start = (this.page - 1) * this.pageSize;
    return this.filtered.slice(start, start + this.pageSize);
  }
  get totalPages() { return Math.ceil(this.filtered.length / this.pageSize); }

  openAdd() { this.form = this.emptyForm(); this.isEdit = false; this.showModal = true; }
  openEdit(s: any) { this.form = { ...s }; this.isEdit = true; this.showModal = true; }
  closeModal() { this.showModal = false; }

  save() {
    if (!this.form.name || !this.form.email) { this.toast.warn('Name and email are required'); return; }
    this.saving = true;
    const call = this.isEdit ? this.api.updateStaff(this.form.id, this.form) : this.api.createStaff(this.form);
    call.subscribe({
      next: () => { this.toast.success(this.isEdit ? 'Staff updated!' : 'Staff added!'); this.showModal = false; this.saving = false; this.load(); },
      error: (err) => { this.toast.error(err?.error?.message || 'Save failed'); this.saving = false; }
    });
  }

  delete(s: any) {
    if (!confirm(`Delete "${s.name}"?`)) return;
    this.api.deleteStaff(s.id).subscribe({
      next: () => { this.toast.success('Staff deleted'); this.load(); },
      error: () => this.toast.error('Delete failed')
    });
  }

  statusClass(status: string) {
    return status === 'Active' ? 'badge-success' : 'badge-warning';
  }
}