import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export type ToastType = 'success' | 'error' | 'info';

export interface ToastMessage {
  id: number;
  type: ToastType;
  message: string;
  duration: number;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private toasts: ToastMessage[] = [];
  private nextId = 1;

  readonly toasts$ = new BehaviorSubject<ToastMessage[]>([]);

  show(message: string, type: ToastType = 'info', duration = 3500): void {
    const toast: ToastMessage = {
      id: this.nextId++,
      type,
      message,
      duration
    };

    this.toasts = [...this.toasts, toast];
    this.toasts$.next(this.toasts);

    window.setTimeout(() => this.remove(toast.id), duration);
  }

  remove(id: number): void {
    this.toasts = this.toasts.filter((toast) => toast.id !== id);
    this.toasts$.next(this.toasts);
  }
}
