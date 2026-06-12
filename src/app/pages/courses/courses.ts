import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CourseService } from '../../core/services/course.service';

@Component({
  selector: 'app-courses',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './courses.html',
  styleUrls: ['./courses.css']
})
export class Courses implements OnInit {

  showModal = false;

  searchText = '';

  course = {
    courseName: '',
    courseCode: '',
    fee: 0,
    durationMonths: 0,
    description: '',
    instituteId: 1
  };

  courses: any[] = [];

  constructor(
    private router: Router,
    private courseService: CourseService
  ) { }

  ngOnInit(): void {
    this.loadCourses();
  }

  loadCourses() {
    this.courseService.getCourses()
      .subscribe(data => {
        this.courses = data;
      });
  }

  openModal() {
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  addCourse() {

    if (
      !this.course.courseName ||
      !this.course.courseCode ||
      !this.course.description ||
      this.course.fee <= 0 ||
      this.course.durationMonths <= 0
    ) {
      alert('Please fill all required fields');
      return;
    }

    this.courseService.createCourse(this.course)
      .subscribe(() => {
        this.loadCourses();
        this.closeModal();
      });
  }

  openCourse(id: number) {
    this.router.navigate(['/courses', id]);
  }
}