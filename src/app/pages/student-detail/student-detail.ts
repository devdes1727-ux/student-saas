import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-student-detail',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './student-detail.html',
  styleUrls: ['./student-detail.css']
})
export class StudentDetail {

  selectedTab = 'overview';

  student = {
    name: 'Ravi Kumar',
    studentId: 'ST001',
    phone: '9876543210',
    parentName: 'Kumar',
    parentPhone: '9876543211',
    course: 'Maths',
    batch: 'Morning Batch',
    status: 'Active'
  };

  courseFee = 5000;

  payments = [
    {
      date: '01-Jun-2026',
      amount: 1000
    },
    {
      date: '10-Jun-2026',
      amount: 2000
    }
  ];

  showPaymentModal = false;

  paymentAmount = 0;

  attendanceSummary = {
    totalClasses: 50,
    attendedClasses: 46
  };

  get totalPaid(): number {
    return this.payments.reduce(
      (sum, payment) => sum + payment.amount,
      0
    );
  }

  get balance(): number {
    return this.courseFee - this.totalPaid;
  }

  get attendancePercentage(): number {
    return Math.round(
      (this.attendanceSummary.attendedClasses /
        this.attendanceSummary.totalClasses) * 100
    );
  }

  openPaymentModal(): void {
    this.showPaymentModal = true;
  }

  closePaymentModal(): void {
    this.showPaymentModal = false;
  }

  addPayment(): void {

    if (!this.paymentAmount || this.paymentAmount <= 0) {
      return;
    }

    this.payments.push({
      date: new Date().toLocaleDateString(),
      amount: this.paymentAmount
    });

    this.paymentAmount = 0;

    this.closePaymentModal();
  }
}