import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-admin-events',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './events.html',
  styleUrls: ['./events.css']
})
export class AdminEvents implements OnInit {
  events: any[] = [];
  loading = true;

  // Form states
  showForm = false;
  editingId: number | null = null;
  formModel = {
    id: 0,
    title: '',
    description: '',
    category: 'Exhibitions',
    eventDate: '',
    location: '',
    imageUrl: ''
  };

  categories = ['Exhibitions', 'Competitions', 'Social Awareness', 'Classes & Workshops', 'Awards & Honors'];

  constructor(private api: ApiService, private toast: ToastService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.loadEvents();
  }

  loadEvents() {
    this.loading = true;
    this.api.getEvents().subscribe({
      next: (data) => {
        this.events = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toast.error('Failed to load events');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  openNewForm() {
    this.editingId = null;
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    this.formModel = {
      id: 0,
      title: '',
      description: '',
      category: 'Exhibitions',
      eventDate: tomorrow.toISOString().split('T')[0],
      location: 'Main School Campus',
      imageUrl: ''
    };
    this.showForm = true;
  }

  openEditForm(ev: any) {
    this.editingId = ev.id;
    this.formModel = {
      id: ev.id,
      title: ev.title,
      description: ev.description || '',
      category: ev.category || 'Exhibitions',
      eventDate: ev.eventDate ? new Date(ev.eventDate).toISOString().split('T')[0] : '',
      location: ev.location || '',
      imageUrl: ev.imageUrl || ''
    };
    this.showForm = true;
  }

  closeForm() {
    this.showForm = false;
    this.editingId = null;
  }

  saveEvent() {
    if (!this.formModel.title.trim()) {
      this.toast.error('Title is required');
      return;
    }
    if (!this.formModel.eventDate) {
      this.toast.error('Event date is required');
      return;
    }

    const payload = {
      ...this.formModel,
      eventDate: new Date(this.formModel.eventDate).toISOString()
    };

    if (this.editingId) {
      this.api.updateEvent(this.editingId, payload).subscribe({
        next: () => {
          this.toast.success('Event updated successfully');
          this.showForm = false;
          this.loadEvents();
        },
        error: () => this.toast.error('Failed to update event')
      });
    } else {
      this.api.createEvent(payload).subscribe({
        next: () => {
          this.toast.success('Event created successfully');
          this.showForm = false;
          this.loadEvents();
        },
        error: () => this.toast.error('Failed to create event')
      });
    }
  }

  deleteEvent(id: number) {
    if (confirm('Are you sure you want to delete this event?')) {
      this.api.deleteEvent(id).subscribe({
        next: () => {
          this.toast.success('Event deleted successfully');
          this.loadEvents();
        },
        error: () => this.toast.error('Failed to delete event')
      });
    }
  }
}
