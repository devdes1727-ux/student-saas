import { Component, OnInit } from '@angular/core';
import { isPlatformBrowser, CommonModule } from '@angular/common';
import { PLATFORM_ID, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ToastService } from './core/services/toast.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App implements OnInit {

  private platformId = inject(PLATFORM_ID);
  public toastService = inject(ToastService);

  ngOnInit() {
    // Theme setup from settings
    if (isPlatformBrowser(this.platformId)) {
      const userTheme = localStorage.getItem('theme') || 'light';
      document.documentElement.setAttribute('data-theme', userTheme);
    }
  }
}