import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';


@Component({
  selector: 'app-students',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './students.html',
  styleUrls: ['./students.css'],
})
export class Students implements OnInit {

  students: any[] = [];
  filtered: any[] = [];
  courses: any[] = [];
  loading = false;
  searchTerm = '';
  filterStatus = '';
  filterCourse = '';
  Math = Math;

  // Pagination
  page = 1;
  pageSize = 10;

  // Modal
  showModal = false;
  isEdit = false;
  saving = false;
  form: any = this.emptyForm();

  constructor(
    private api: ApiService,
    private toast: ToastService,
    private cdr: ChangeDetectorRef) { }

  ngOnInit() {
    this.loadStudents();
    this.api.getCourses().subscribe({ next: (d: any) => this.courses = d });
  }

  emptyForm() {
    return {
      id: null, name: '', email: '', phone: '', address: '',
      courseId: '', enrollmentDate: '', status: 'Active',
      guardianName: '', guardianPhone: ''
    };
  }

  loadStudents() {
    this.loading = true;
    this.api.getStudents().subscribe({
      next: (data: any) => {
        this.students = Array.isArray(data) ? data : (data.items || []);
        this.applyFilter();
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => { this.toast.error('Failed to load students'); this.loading = false; }
    });
  }

  applyFilter() {
    const term = this.searchTerm.toLowerCase();
    this.filtered = this.students.filter(s =>
      (!term || s.name?.toLowerCase().includes(term) || s.email?.toLowerCase().includes(term) || s.phone?.includes(term)) &&
      (!this.filterStatus || s.status === this.filterStatus) &&
      (!this.filterCourse || String(s.courseId) === String(this.filterCourse))
    );
    this.page = 1;
  }

  get paginated() {
    const start = (this.page - 1) * this.pageSize;
    return this.filtered.slice(start, start + this.pageSize);
  }

  get totalPages() { return Math.ceil(this.filtered.length / this.pageSize); }

  openAdd() { this.form = this.emptyForm(); this.isEdit = false; this.showModal = true; }

  openEdit(s: any) {
    this.form = { ...s, enrollmentDate: s.enrollmentDate?.substring(0, 10) };
    this.isEdit = true;
    this.showModal = true;
  }

  closeModal() { this.showModal = false; }

  save() {
    if (!this.form.name || !this.form.email) {
      this.toast.warn('Name and email are required'); return;
    }
    this.saving = true;
    const call = this.isEdit
      ? this.api.updateStudent(this.form.id, this.form)
      : this.api.createStudent(this.form);

    call.subscribe({
      next: () => {
        this.toast.success(this.isEdit ? 'Student updated!' : 'Student added!');
        this.showModal = false;
        this.saving = false;
        this.loadStudents();
      },
      error: (err) => {
        this.toast.error(err?.error?.message || 'Save failed');
        this.saving = false;
      }
    });
  }

  deleteStudent(s: any) {
    if (!confirm(`Delete "${s.name}"? This cannot be undone.`)) return;
    this.api.deleteStudent(s.id).subscribe({
      next: () => { this.toast.success('Student deleted'); this.loadStudents(); },
      error: () => this.toast.error('Delete failed')
    });
  }

  statusClass(status: string) {
    const map: any = { Active: 'badge-success', Inactive: 'badge-warning', Dropped: 'badge-danger' };
    return map[status] || 'badge-info';
  }
}