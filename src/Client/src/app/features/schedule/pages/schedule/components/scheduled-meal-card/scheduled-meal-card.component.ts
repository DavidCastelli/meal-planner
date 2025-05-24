import {
  Component,
  EventEmitter,
  HostListener,
  inject,
  Input,
  Output,
} from '@angular/core';
import { NgOptimizedImage } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-scheduled-meal-card',
  standalone: true,
  imports: [NgOptimizedImage],
  templateUrl: './scheduled-meal-card.component.html',
  styleUrl: './scheduled-meal-card.component.css',
})
export class ScheduledMealCardComponent {
  private readonly router = inject(Router);

  @Input({ required: true }) mealId!: number;
  @Input({ required: true }) title!: string;
  @Input() imageUrl?: string;

  @Output() deleted = new EventEmitter<number>();

  @HostListener('click', ['$event'])
  showMealDetails(event: Event) {
    // Prevents the cards action button from triggering navigation to details page.
    if (event.target instanceof HTMLButtonElement) {
      return;
    }

    void this.router.navigate([`/manage/meals/${this.mealId}/details`]);
  }

  remove() {
    this.deleted.emit(this.mealId);
  }
}
