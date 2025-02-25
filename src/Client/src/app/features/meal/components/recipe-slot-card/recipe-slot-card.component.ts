import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgOptimizedImage } from '@angular/common';

@Component({
  selector: 'app-recipe-slot-card',
  standalone: true,
  imports: [NgOptimizedImage],
  templateUrl: './recipe-slot-card.component.html',
  styleUrl: './recipe-slot-card.component.css',
})
export class RecipeSlotCardComponent {
  @Input({ required: true }) title!: string;
  @Input() imageUrl?: string;

  @Output() removed = new EventEmitter<boolean>();

  remove() {
    return this.removed.emit(true);
  }
}
