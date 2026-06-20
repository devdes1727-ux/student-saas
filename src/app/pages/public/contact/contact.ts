import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-public-contact',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './contact.html',
  styleUrls: ['./contact.css']
})
export class Contact {
  contactForm: FormGroup;
  submitted = false;
  success = false;
  error = false;

  constructor(private fb: FormBuilder, private api: ApiService, private cdr: ChangeDetectorRef) {
    this.contactForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', Validators.required],
      message: ['', Validators.required]
    });
  }

  submitForm() {
    this.submitted = true;

    if (this.contactForm.invalid) {
      return;
    }

    const payload = this.contactForm.value;
    this.api.submitContact(payload).subscribe({
      next: () => {
        this.success = true;
        this.error = false;
        this.contactForm.reset();
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
