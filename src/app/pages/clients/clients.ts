import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-clients',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './clients.html',
  styleUrls: ['./clients.css']
})
export class Clients {

  clients = [
    {
      name: 'ABC Academy',
      plan: 'Growth',
      students: 250,
      status: 'Active'
    },
    {
      name: 'Smart Tuition',
      plan: 'Starter',
      students: 75,
      status: 'Trial'
    }
  ];

}