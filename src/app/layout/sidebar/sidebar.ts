import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { PLATFORM_ID, inject } from '@angular/core';
import { SessionService } from '../../core/services/session.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterModule, CommonModule],
  templateUrl: './sidebar.html',
  styleUrls: ['./sidebar.css']
})
export class Sidebar implements OnInit {

  private platformId = inject(PLATFORM_ID);

  user: any = {};

  constructor(
  private router: Router,
  public session: SessionService
) {}

  ngOnInit() {

    if (isPlatformBrowser(this.platformId)) {

      const userData = localStorage.getItem('user');

      if (userData) {
          this.user = this.session.getUser();

      }

    }
  }

  logout() {

    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem('user');
    }

    this.router.navigate(['/login']);
  }
}