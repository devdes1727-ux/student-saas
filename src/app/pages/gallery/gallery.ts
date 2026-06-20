import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-gallery',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './gallery.html',
  styleUrls: ['./gallery.css']
})
export class Gallery implements OnInit {

  items: any[] = [];
  filtered: any[] = [];
  loading = false;
  filterCategory = '';
  categories = ['All', 'Student Work', 'Events', 'Competitions', 'Classes', 'Awards'];

  showModal = false;
  isEdit = false;
  saving = false;
  form: any = this.emptyForm();

  constructor(
    private api: ApiService,
    private toast: ToastService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() { this.load(); }

  emptyForm() {
    return { id: null, title: '', description: '', imageUrl: '', category: 'Student Work', isFeatured: false };
  }

  load() {
    this.loading = true;
    this.api.getGallery().subscribe({
      next: (d: any) => {
        this.items = Array.isArray(d) ? d : [];
        this.applyFilter();
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toast.error('Failed to load gallery');
        this.loading = false;
      }
    });
  }

  applyFilter() {
    this.filtered = this.filterCategory && this.filterCategory !== 'All'
      ? this.items.filter(i => i.category === this.filterCategory)
      : [...this.items];
  }

  openAdd() { this.form = this.emptyForm(); this.isEdit = false; this.showModal = true; }
  openEdit(item: any) { this.form = { ...item }; this.isEdit = true; this.showModal = true; }
  closeModal() { this.showModal = false; }

  save() {
    if (!this.form.title || !this.form.imageUrl) { this.toast.warn('Title and image URL are required'); return; }
    this.saving = true;
    const call = this.isEdit ? this.api.updateGalleryItem(this.form.id, this.form) : this.api.createGalleryItem(this.form);
    call.subscribe({
      next: () => { this.toast.success(this.isEdit ? 'Updated!' : 'Added to gallery!'); this.showModal = false; this.saving = false; this.load(); },
      error: () => { this.toast.error('Save failed'); this.saving = false; }
    });
  }

  delete(item: any) {
    if (!confirm(`Remove "${item.title}" from gallery?`)) return;
    this.api.deleteGalleryItem(item.id).subscribe({
      next: () => { this.toast.success('Removed'); this.load(); },
      error: () => this.toast.error('Delete failed')
    });
  }
}
