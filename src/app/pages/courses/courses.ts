import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-courses',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './courses.html',
  styleUrls: ['./courses.css']
})
export class Courses implements OnInit {

  courses: any[] = [];
  filtered: any[] = [];

  loading = false;
  searchTerm = '';

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
  }

  emptyForm() {
    return {
      id: null,
      name: '',
      description: '',
      fee: '',
      duration: '',
      level: 'Beginner',
      ageGroup: '',
      maxStudents: 20,
      isActive: true
    };
  }

  load() {
    this.loading = true;

    this.api.getCourses().subscribe({
      next: (d: any) => {

        const data = Array.isArray(d)
          ? d
          : (d.items || []);

        this.courses = data.map((c: any) => ({
          id: c.id,
          name: c.name ?? c.courseName,
          description: c.description ?? '',
          fee: c.fee ?? 0,
          duration: c.duration ?? c.durationMonths ?? '',
          level: c.level ?? c.category ?? 'Beginner',
          ageGroup: c.ageGroup ?? 'All Ages',
          maxStudents: c.maxStudents ?? 20,
          isActive: c.isActive ?? true
        }));

        this.filtered = [...this.courses];

        this.loading = false;
        this.cdr.detectChanges();

        console.log(this.filtered);
      },
      error: () => {
        this.toast.error('Failed to load courses');
        this.loading = false;
      }
    });
  }

  applyFilter() {

    if (!this.searchTerm) {
      this.filtered = [...this.courses];
      return;
    }

    const term = this.searchTerm.toLowerCase();

    this.filtered = this.courses.filter(c =>
      c.name?.toLowerCase().includes(term)
    );
  }

  openAdd() {
    this.form = this.emptyForm();
    this.isEdit = false;
    this.showModal = true;
  }

  openEdit(c: any) {

    this.form = {
      id: c.id,
      name: c.name,
      description: c.description,
      fee: c.fee,
      duration: c.duration,
      level: c.level,
      ageGroup: c.ageGroup,
      maxStudents: c.maxStudents,
      isActive: c.isActive
    };

    this.isEdit = true;
    this.showModal = true;

  }

  save() {

    if (!this.form.name) {
      this.toast.warn('Course name is required');
      return;
    }

    this.saving = true;

    const payload = {
      id: this.form.id,
      courseName: this.form.name,
      courseCode: '',
      description: this.form.description,
      fee: Number(this.form.fee),
      durationMonths: parseInt(this.form.duration) || 0,
      category: this.form.level,
      instituteId: 1,
      isActive: this.form.isActive
    };

    const req = this.isEdit
      ? this.api.updateCourse(this.form.id, payload)
      : this.api.createCourse(payload);

    req.subscribe({
      next: () => {
        this.toast.success(this.isEdit ? 'Course updated' : 'Course created');
        this.showModal = false;
        this.saving = false;
        this.load();
      },
      error: (err) => {
        console.log(err);
        this.toast.error('Save failed');
        this.saving = false;
      }
    });

  }
  closeModal() {
    this.showModal = false;
  }
  delete(c: any) {

    if (!confirm(`Delete "${c.name}" ?`)) {
      return;
    }

    this.api.deleteCourse(c.id).subscribe({
      next: () => {
        this.toast.success('Course deleted');
        this.load();
      },
      error: (e) => {
        this.toast.error(e?.error?.message || 'Delete failed');
      }
    });

  }

}