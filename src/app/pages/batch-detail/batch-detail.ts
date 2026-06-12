import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-batch-detail',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './batch-detail.html',
  styleUrls: ['./batch-detail.css']
})
export class BatchDetail {

  selectedTab = 'students';

  batch = {
    name: 'Morning Batch',
    course: 'Mathematics',
    timing: '7:00 AM - 8:30 AM',
    staff: 'Mr. Kumar'
  };

  students = [
    {
      id: 1,
      name: 'Ravi',
      phone: '9876543210',
      present: true
    },
    {
      id: 2,
      name: 'Priya',
      phone: '9876543211',
      present: false
    },
    {
      id: 3,
      name: 'Arun',
      phone: '9876543212',
      present: true
    }
  ];

  saveAttendance() {

    const presentCount =
      this.students.filter(x => x.present).length;

    alert(
      `Attendance Saved\nPresent: ${presentCount}/${this.students.length}`
    );
  }

}