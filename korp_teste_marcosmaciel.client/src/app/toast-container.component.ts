import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

import { ToastMessage, ToastService } from './services/toast.service';

@Component({
  selector: 'app-toast-container',
  template: `
    <div class="toast-container" *ngIf="toasts.length > 0">
      <div *ngFor="let toast of toasts" class="toast toast-{{ toast.type }}" role="status" aria-live="polite">
        <span>{{ toast.message }}</span>
        <button type="button" class="toast-close" (click)="dismiss(toast.id)" aria-label="Fechar mensagem">×</button>
      </div>
    </div>
  `,
  styles: [
    `
      .toast-container {
        position: fixed;
        top: 1rem;
        right: 1rem;
        z-index: 1000;
        display: flex;
        flex-direction: column;
        gap: 0.75rem;
        max-width: min(360px, calc(100vw - 2rem));
      }

      .toast {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 1rem;
        border-radius: 12px;
        padding: 0.9rem 1rem;
        font-weight: 600;
        box-shadow: 0 16px 30px rgba(15, 23, 42, 0.16);
        color: #111827;
        background: #fff;
        border: 1px solid rgba(148, 163, 184, 0.3);
      }

      .toast-success {
        background: #dcfce7;
        border-color: #86efac;
        color: #166534;
      }

      .toast-error {
        background: #fee2e2;
        border-color: #fca5a5;
        color: #991b1b;
      }

      .toast-info {
        background: #dbeafe;
        border-color: #93c5fd;
        color: #1d4ed8;
      }

      .toast-close {
        border: none;
        background: transparent;
        color: inherit;
        font-size: 1.25rem;
        line-height: 1;
        cursor: pointer;
        opacity: 0.8;
      }
    `
  ],
  standalone: false
})
export class ToastContainerComponent implements OnInit, OnDestroy {
  toasts: ToastMessage[] = [];
  private subscription?: Subscription;

  constructor(private toastService: ToastService) {}

  ngOnInit(): void {
    this.subscription = this.toastService.toasts$.subscribe((toasts) => {
      this.toasts = toasts;
    });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  dismiss(id: number): void {
    this.toastService.remove(id);
  }
}
