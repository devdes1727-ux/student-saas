import { Injectable } from '@angular/core';

export interface Toast {
  message: string;
  type: 'success' | 'warning' | 'error' | 'info';
  id: number;
}

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  toasts: Toast[] = [];
  private counter = 0;

  show(message: string, type: 'success' | 'warning' | 'error' | 'info' = 'info') {
    const id = this.counter++;
    const toast: Toast = { message, type, id };
    this.toasts.push(toast);

    // Auto-remove after 4 seconds
    setTimeout(() => {
      this.remove(id);
    }, 4000);
  }

  success(message: string) {
    this.show(message, 'success');
  }

  warning(message: string) {
    this.show(message, 'warning');
  }


  warn(message: string) {
    this.warning(message);
  }

  error(message: string) {
    this.show(message, 'error');
  }

  info(message: string) {
    this.show(message, 'info');
  }

  remove(id: number) {
    this.toasts = this.toasts.filter(t => t.id !== id);
  }
}
