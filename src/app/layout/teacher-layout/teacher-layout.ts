import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Sidebar } from '../sidebar/sidebar';

@Component({
  selector: 'app-teacher-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, Sidebar],
  templateUrl: './teacher-layout.html',
  styleUrls: ['./teacher-layout.css']
})
export class TeacherLayout {}
