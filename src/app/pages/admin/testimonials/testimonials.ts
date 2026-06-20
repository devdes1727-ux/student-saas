import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-admin-testimonials',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './testimonials.html',
  styleUrls: ['./testimonials.css']
})
export class AdminTestimonials implements OnInit {
  testimonials: any[] = [];
  loading = true;

  // Form states
  showForm = false;
  editingId: number | null = null;
  formModel = {
    id: 0,
    name: '',
    role: 'Parent',
    message: '',
    rating: 5,
    imageUrl: ''
  };

  ratings = [5, 4, 3, 2, 1];

  constructor(private api: ApiService, private toast: ToastService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.loadTestimonials();
  }

  loadTestimonials() {
    this.loading = true;
    this.api.getAdminTestimonials().subscribe({
      next: (data) => {
        this.testimonials = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toast.error('Failed to load testimonials');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  openNewForm() {
    this.editingId = null;
    this.formModel = {
      id: 0,
      name: '',
      role: 'Parent',
      message: '',
      rating: 5,
      imageUrl: ''
    };
    this.showForm = true;
  }

  openEditForm(item: any) {
    this.editingId = item.id;
    this.formModel = {
      id: item.id,
      name: item.name,
      role: item.role || 'Parent',
      message: item.message,
      rating: item.rating || 5,
      imageUrl: item.imageUrl || ''
    };
    this.showForm = true;
  }

  closeForm() {
    this.showForm = false;
    this.editingId = null;
  }

  saveTestimonial() {
    if (!this.formModel.name.trim()) {
      this.toast.error('Name is required');
      return;
    }
    if (!this.formModel.message.trim()) {
      this.toast.error('Message is required');
      return;
    }

    const payload = {
      ...this.formModel,
      rating: Number(this.formModel.rating)
    };

    if (this.editingId) {
      this.api.updateTestimonial(this.editingId, payload).subscribe({
        next: () => {
          this.toast.success('Testimonial updated successfully');
          this.showForm = false;
          this.loadTestimonials();
        },
        error: () => this.toast.error('Failed to update testimonial')
      });
    } else {
      this.api.createTestimonial(payload).subscribe({
        next: () => {
          this.toast.success('Testimonial added successfully');
          this.showForm = false;
          this.loadTestimonials();
        },
        error: () => this.toast.error('Failed to add testimonial')
      });
    }
  }

  deleteTestimonial(id: number) {
    if (confirm('Are you sure you want to delete this testimonial?')) {
      this.api.deleteTestimonial(id).subscribe({
        next: () => {
          this.toast.success('Testimonial deleted successfully');
          this.loadTestimonials();
        },
        error: () => this.toast.error('Failed to delete testimonial')
      });
    }
  }
}
