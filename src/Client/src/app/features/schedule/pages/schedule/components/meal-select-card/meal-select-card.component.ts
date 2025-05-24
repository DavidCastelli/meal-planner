import { Component, Input } from '@angular/core';
import { NgOptimizedImage } from '@angular/common';

@Component({
  selector: 'app-meal-select-card',
  standalone: true,
  imports: [NgOptimizedImage],
  templateUrl: './meal-select-card.component.html',
  styleUrl: './meal-select-card.component.css',
})
export class MealSelectCardComponent {
  @Input({ required: true }) title!: string;
  @Input() imageUrl!: string;
}
