import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-admin-batches',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './batches.html',
  styleUrls: ['./batches.css']
})
export class AdminBatches implements OnInit {
  batches: any[] = [];
  courses: any[] = [];
  teachers: any[] = [];
  loading = true;

  // Form states
  showForm = false;
  editingId: number | null = null;
  formModel = {
    id: 0,
    batchName: '',
    startTime: '',
    endTime: '',
    teacherId: null as number | null,
    courseId: null as number | null,
    isActive: true
  };

  constructor(private api: ApiService, private toast: ToastService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading = true;
    this.api.getBatches().subscribe({
      next: (batchesData) => {
        this.batches = batchesData;
        this.loadCoursesAndTeachers();
      },
      error: () => {
        this.toast.error('Failed to load batches');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadCoursesAndTeachers() {
    this.api.getCourses().subscribe({
      next: (coursesData) => {
        this.courses = coursesData;
        this.api.getStaff().subscribe({
          next: (staffData) => {
            // Filter only teachers
            this.teachers = staffData.filter((s: any) => s.role === 'Teacher' && s.activeStatus);
            this.loading = false;
            this.cdr.detectChanges();
          },
          error: () => {
            this.toast.error('Failed to load staff list');
            this.loading = false;
            this.cdr.detectChanges();
          }
        });
      },
      error: () => {
        this.toast.error('Failed to load courses');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  openNewForm() {
    this.editingId = null;
    this.formModel = {
      id: 0,
      batchName: '',
      startTime: '09:00',
      endTime: '11:00',
      teacherId: this.teachers.length > 0 ? this.teachers[0].id : null,
      courseId: this.courses.length > 0 ? this.courses[0].id : null,
      isActive: true
    };
    this.showForm = true;
  }

  openEditForm(batch: any) {
    this.editingId = batch.id;
    this.formModel = {
      id: batch.id,
      batchName: batch.batchName,
      startTime: batch.startTime || '09:00',
      endTime: batch.endTime || '11:00',
      teacherId: batch.teacherId,
      courseId: batch.courseId,
      isActive: batch.isActive
    };
    this.showForm = true;
  }

  closeForm() {
    this.showForm = false;
    this.editingId = null;
  }

  saveBatch() {
    if (!this.formModel.batchName.trim()) {
      this.toast.error('Batch Name is required');
      return;
    }
    if (!this.formModel.courseId) {
      this.toast.error('Course selection is required');
      return;
    }
    if (!this.formModel.teacherId) {
      this.toast.error('Teacher selection is required');
      return;
    }

    const payload = {
      ...this.formModel,
      courseId: Number(this.formModel.courseId),
      teacherId: Number(this.formModel.teacherId)
    };

    if (this.editingId) {
      this.api.updateBatch(this.editingId, payload).subscribe({
        next: () => {
          this.toast.success('Batch updated successfully');
          this.showForm = false;
          this.loadData();
        },
        error: () => this.toast.error('Failed to update batch')
      });
    } else {
      this.api.createBatch(payload).subscribe({
        next: () => {
          this.toast.success('Batch created successfully');
          this.showForm = false;
          this.loadData();
        },
        error: () => this.toast.error('Failed to create batch')
      });
    }
  }

  deleteBatch(id: number) {
    if (confirm('Are you sure you want to delete this batch?')) {
      this.api.deleteBatch(id).subscribe({
        next: () => {
          this.toast.success('Batch deleted successfully');
          this.loadData();
        },
        error: () => this.toast.error('Failed to delete batch')
      });
    }
  }
}
