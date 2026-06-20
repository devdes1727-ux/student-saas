import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-attendance',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './attendance.html',
  styleUrls: ['./attendance.css']
})
export class Attendance implements OnInit {

  courses: any[] = [];
  selectedCourse = '';
  selectedDate = new Date().toISOString().substring(0, 10);
  students: any[] = [];
  attendanceMap: { [studentId: number]: string } = {};
  loading = false;
  saving = false;

  // Monthly view
  monthlyView = false;
  selectedStudent = '';
  selectedMonth = new Date().getMonth() + 1;
  selectedYear = new Date().getFullYear();
  monthlyRecords: any[] = [];
  years = Array.from({ length: 5 }, (_, i) => new Date().getFullYear() - i);
  months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];

  constructor(
    private api: ApiService,
    private toast: ToastService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.api.getCourses().subscribe({
      next: (d: any) => {
        this.courses = Array.isArray(d) ? d : (d.items || []);
        this.cdr.detectChanges();
      }
    });
  }

  loadAttendance() {
    if (!this.selectedCourse || !this.selectedDate) { this.toast.warn('Select course and date'); return; }
    this.loading = true;
    this.api.getAttendance(+this.selectedCourse, this.selectedDate).subscribe({
      next: (data: any) => {
        this.students = data.students || [];
        this.attendanceMap = {};
        this.students.forEach((s: any) => {
          this.attendanceMap[s.id] = data.attendance?.[s.id] || 'Absent';
        });
        this.loading = false;
      },
      error: () => { this.toast.error('Failed to load attendance'); this.loading = false; }
    });
  }

  markAll(status: string) {
    this.students.forEach(s => this.attendanceMap[s.id] = status);
  }

  save() {
    if (!this.students.length) { this.toast.warn('No students loaded'); return; }
    this.saving = true;
    const payload = {
      courseId: +this.selectedCourse,
      date: this.selectedDate,
      records: this.students.map(s => ({ studentId: s.id, status: this.attendanceMap[s.id] || 'Absent' }))
    };
    this.api.saveAttendance(payload).subscribe({
      next: () => { this.toast.success('Attendance saved!'); this.saving = false; },
      error: () => { this.toast.error('Save failed'); this.saving = false; }
    });
  }

  loadMonthly() {
    if (!this.selectedStudent) { this.toast.warn('Select a student'); return; }
    this.loading = true;
    this.api.getMonthlyAttendance(+this.selectedStudent, this.selectedMonth, this.selectedYear).subscribe({
      next: (data: any) => { this.monthlyRecords = data || []; this.loading = false; },
      error: () => { this.toast.error('Failed to load monthly records'); this.loading = false; }
    });
  }

  exportCsv() {
    if (!this.selectedCourse) return;
    this.api.exportAttendanceCsv(+this.selectedCourse, this.selectedMonth, this.selectedYear).subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a'); a.href = url;
        a.download = `attendance_${this.selectedMonth}_${this.selectedYear}.csv`;
        a.click(); window.URL.revokeObjectURL(url);
        this.toast.success('CSV downloaded!');
      },
      error: () => this.toast.error('Export failed')
    });
  }

  statusClass(status: string) {
    const map: any = { Present: 'badge-success', Absent: 'badge-danger', Late: 'badge-warning', Leave: 'badge-info' };
    return map[status] || 'badge-info';
  }

  get allStudents() { return this.courses.flatMap((c: any) => c.students || []); }
}
