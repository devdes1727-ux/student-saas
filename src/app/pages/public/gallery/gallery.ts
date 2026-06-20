import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-public-gallery',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './gallery.html',
  styleUrls: ['./gallery.css']
})
export class Gallery implements OnInit {
  gallery: any[] = [];
  filtered: any[] = [];
  categories = ['All', 'Student Work', 'Events', 'Competitions', 'Classes', 'Awards'];
  activeCategory = 'All';
  loading = true;

  constructor(private api: ApiService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.api.getPublicGallery().subscribe({
      next: (data) => {
        this.gallery = data;
        this.filtered = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  filterCategory(cat: string) {
    this.activeCategory = cat;
    if (cat === 'All') {
      this.filtered = this.gallery;
    } else {
      this.filtered = this.gallery.filter(g => g.category.toLowerCase() === cat.toLowerCase());
    }
    this.cdr.detectChanges();
  }
}
