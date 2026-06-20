import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-public-courses',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './courses.html',
  styleUrls: ['./courses.css']
})
export class Courses implements OnInit {
  courses: any[] = [];
  loading = true;

  constructor(private api: ApiService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.api.getPublicCourses().subscribe({
      next: (data) => {
        this.courses = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }
}
