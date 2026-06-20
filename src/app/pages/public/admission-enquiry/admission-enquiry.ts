import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-admission-enquiry',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './admission-enquiry.html',
  styleUrls: ['./admission-enquiry.css']
})
export class AdmissionEnquiry implements OnInit {
  enquiryForm: FormGroup;
  courses: any[] = [];
  submitted = false;
  success = false;
  error = false;

  constructor(private fb: FormBuilder, private api: ApiService, private cdr: ChangeDetectorRef) {
    this.enquiryForm = this.fb.group({
      studentName: ['', Validators.required],
      age: [null, [Validators.required, Validators.min(3), Validators.max(100)]],
      gender: ['Male', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', Validators.required],
      parentName: ['', Validators.required],
      parentPhone: ['', Validators.required],
      courseId: [null, Validators.required],
      message: ['']
    });
  }

  ngOnInit() {
    this.api.getPublicCourses().subscribe({
      next: (data) => {
        this.courses = data;
        if (data.length > 0) {
          this.enquiryForm.patchValue({ courseId: data[0].id });
        }
        this.cdr.detectChanges();
      }
    });
  }

  submitForm() {
    this.submitted = true;

    if (this.enquiryForm.invalid) {
      return;
    }

    const payload = this.enquiryForm.value;
    payload.courseId = Number(payload.courseId);

    this.api.submitEnquiry(payload).subscribe({
      next: () => {
        this.success = true;
        this.error = false;
        this.enquiryForm.reset();
        this.submitted = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = true;
        this.success = false;
        this.cdr.detectChanges();
      }
    });
  }
}
