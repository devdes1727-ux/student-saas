import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-public-events',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './events.html',
  styleUrls: ['./events.css']
})
export class Events implements OnInit {
  events: any[] = [];
  loading = true;

  constructor(private api: ApiService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.api.getPublicEvents().subscribe({
      next: (data) => {
        this.events = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }
}
