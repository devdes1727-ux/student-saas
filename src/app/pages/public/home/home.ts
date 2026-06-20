import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-public-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './home.html',
  styleUrls: ['./home.css']
})
export class Home implements OnInit {
  metrics: any[] = [];
  services = [
    { title: 'Drawing Classes', subtitle: 'Fundamentals of form, line and perspective.', icon: '🎨' },
    { title: 'Painting Classes', subtitle: 'Watercolor, acrylic, and mixed-media journeys.', icon: '🖌️' },
    { title: 'Pencil Sketching', subtitle: 'Realistic portraits, landscapes and still life.', icon: '✏️' },
    { title: 'Creative Workshops', subtitle: 'Hands-on sessions for art competitions and campaigns.', icon: '🧠' }
  ];
  featuredCourses: any[] = [];
  events: any[] = [];
  gallery: any[] = [];
  testimonials: any[] = [];

  constructor(private api: ApiService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.loadStats();
    this.loadCourses();
    this.loadEvents();
    this.loadGallery();
    this.loadTestimonials();
  }

  loadStats() {
    this.api.getPublicStats().subscribe({
      next: (data) => {
        this.metrics = [
          { label: 'Events', value: '150+', description: 'Art and awareness campaigns organized.' },
          { label: 'Students', value: data.totalStudents + '+', description: 'Young artists trained with care.' },
          { label: 'Awards', value: 'Multiple', description: 'District and state-level recognition.' },
          { label: 'Years', value: data.yearsExperience + '+', description: 'Creative excellence in Tirunelveli.' }
        ];
        this.cdr.detectChanges();
      },
      error: () => {
        // Fallback
        this.metrics = [
          { label: 'Events', value: '150+', description: 'Art and awareness campaigns.' },
          { label: 'Students', value: '500+', description: 'Young artists trained.' },
          { label: 'Awards', value: 'Multiple', description: 'Recognition.' },
          { label: 'Years', value: '10+', description: 'Creative excellence.' }
        ];
        this.cdr.detectChanges();
      }
    });
  }

  loadCourses() {
    this.api.getPublicCourses().subscribe({
      next: (data) => {
        this.featuredCourses = data.slice(0, 3).map((c: any) => ({
          title: c.courseName,
          summary: c.description,
          duration: c.durationMonths + ' months',
          skillLevel: 'Beginner to Intermediate',
          imageUrl: c.imageUrl || 'assets/student_sketches.webp', // fallback
          features: ['Structured guidance', 'Realism & tone', 'Interactive feedback']
        }));
        this.cdr.detectChanges();
      }
    });
  }

  loadEvents() {
    this.api.getPublicEvents().subscribe({
      next: (data) => {
        this.events = data.slice(0, 3).map((e: any) => ({
          title: e.title,
          description: e.description,
          category: e.category,
          date: new Date(e.eventDate).toLocaleDateString(),
          location: e.location,
          imageUrl: e.imageUrl || 'assets/rotary.webp'
        }));
        this.cdr.detectChanges();
      }
    });
  }

  loadGallery() {
    this.api.getPublicGallery().subscribe({
      next: (data) => {
        this.gallery = data.slice(0, 4).map((g: any) => ({
          title: g.title,
          category: g.category,
          imageUrl: g.imageUrl
        }));
        this.cdr.detectChanges();
      }
    });
  }

  loadTestimonials() {
    this.api.getPublicTestimonials().subscribe({
      next: (data) => {
        this.testimonials = data.slice(0, 2);
        this.cdr.detectChanges();
      }
    });
  }
}
