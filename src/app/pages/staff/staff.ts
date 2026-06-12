import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-staff',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './staff.html',
  styleUrls: ['./staff.css']
})
export class Staff {

  showModal = false;

  staff = {
    name: '',
    role: '',
    phone: ''
  };

  staffList = [
    {
      id: 1,
      name: 'Kumar',
      role: 'Math Teacher',
      phone: '9876543210',
      status: 'Active'
    }
  ];

  constructor(private router: Router) { }

  openModal() {
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  addStaff() {

    this.staffList.push({
      id: Date.now(),
      name: this.staff.name,
      role: this.staff.role,
      phone: this.staff.phone,
      status: 'Active'
    });

    this.staff = {
      name: '',
      role: '',
      phone: ''
    };

    this.closeModal();
  }

  viewStaff(id: number) {
    this.router.navigate(['/staff', id]);
  }
}