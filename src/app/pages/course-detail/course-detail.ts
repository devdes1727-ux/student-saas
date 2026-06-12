import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-course-detail',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './course-detail.html',
  styleUrls: ['./course-detail.css'],
})
export class CourseDetail {

  constructor(private router: Router) { }

  showBatchModal = false;

  course = {
    name: 'Mathematics',
    duration: '6 Months',
    fee: 5000
  };

  batch = {
    name: '',
    timing: ''
  };

  batches = [
    {
      id: 1,
      name: 'Morning Batch',
      timing: '7:00 AM - 8:30 AM',
      students: 25
    },
    {
      id: 2,
      name: 'Evening Batch',
      timing: '6:00 PM - 7:30 PM',
      students: 18
    }
  ];

  openBatch(id: number) {
    this.router.navigate(['/batches', id]);
  }

  openBatchModal() {
    this.showBatchModal = true;
  }

  closeBatchModal() {
    this.showBatchModal = false;
  }

  addBatch() {

    this.batches.push({
      id: Date.now(),
      name: this.batch.name,
      timing: this.batch.timing,
      students: 0
    });

    this.batch = {
      name: '',
      timing: ''
    };

    this.closeBatchModal();
  }
}