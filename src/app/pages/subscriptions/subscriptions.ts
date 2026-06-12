import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-subscriptions',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './subscriptions.html',
  styleUrls: ['./subscriptions.css']
})
export class Subscriptions {

  plans = [
    {
      name: 'Starter',
      price: 499,
      students: 100
    },
    {
      name: 'Growth',
      price: 999,
      students: 500
    },
    {
      name: 'Pro',
      price: 1999,
      students: 'Unlimited'
    }
  ];

}