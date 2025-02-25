import { Component, Input } from '@angular/core';
import { NgOptimizedImage } from '@angular/common';
import { CdkDragPlaceholder } from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-recipe-select-card',
  standalone: true,
  imports: [NgOptimizedImage, CdkDragPlaceholder],
  templateUrl: './recipe-select-card.component.html',
  styleUrl: './recipe-select-card.component.css',
})
export class RecipeSelectCardComponent {
  @Input({ required: true }) title!: string;
  @Input() imageUrl?: string;
}
