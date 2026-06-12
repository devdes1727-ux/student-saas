import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StudentService } from '../../core/services/student.service';

@Component({
  selector: 'app-students',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './students.html',
  styleUrls: ['./students.css']
})
export class Students {

  showModal = false;

  student = {
    name: '',
    course: '',
    batch: '',
    phone: '',
    parentName: '',
    parentPhone: '',
  };
  students: any[] = [];


  constructor(private router: Router, private studentService: StudentService) { }


  ngOnInit() {
    this.loadStudents();
  }

  loadStudents() {
    this.studentService.getStudents()
      .subscribe(data => {
        this.students = data;
      });
  }
  openModal() {
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  addStudent() {

    if (
      !this.student.name ||
      !this.student.phone ||
      !this.student.parentName ||
      !this.student.parentPhone
    ) {
      alert('Please fill all fields');
      return;
    }
    this.studentService.addStudent({
      name: this.student.name,
      phone: this.student.phone,
      parentName: this.student.parentName,
      parentPhone: this.student.parentPhone,
      course: this.student.course,
      batch: this.student.batch,
      instituteId: 1
    })
      .subscribe({
        next: () => {

          this.loadStudents();

          this.student = {
            name: '',
            course: '',
            batch: '',
            phone: '',
            parentName: '',
            parentPhone: '',
          };

          this.closeModal();
        },

        error: (err) => {
          console.error(err);
          alert('Failed to save student');
        }
      });

  }

  viewStudent(id: number) {
    this.router.navigate(['/students', id]);
  }

}