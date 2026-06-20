import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-admin-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './settings.html',
  styleUrls: ['./settings.css']
})
export class AdminSettings implements OnInit {
  settings: any = {
    id: 1,
    name: '',
    phone: '',
    logoUrl: '',
    themeSettings: 'light',
    emailSettings: '',
    whatsAppNumber: '',
    address: '',
    socialMediaLinks: ''
  };
  loading = true;

  constructor(private api: ApiService, private toast: ToastService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.loadSettings();
  }

  loadSettings() {
    this.loading = true;
    this.api.getSettings().subscribe({
      next: (data) => {
        this.settings = {
          ...this.settings,
          ...data
        };
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toast.error('Failed to load institute settings');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  saveSettings() {
    if (!this.settings.name.trim()) {
      this.toast.error('Academy Name is required');
      return;
    }

    this.api.updateSettings(this.settings).subscribe({
      next: (data) => {
        this.settings = data;
        this.toast.success('Academy settings updated successfully');
      },
      error: () => this.toast.error('Failed to update settings')
    });
  }
}
