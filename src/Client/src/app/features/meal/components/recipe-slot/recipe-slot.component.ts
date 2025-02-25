import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { RecipeSlotCardComponent } from '../recipe-slot-card/recipe-slot-card.component';
import { RecipeSelectService } from '../../recipe-select.service';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-recipe-slot',
  standalone: true,
  imports: [RecipeSlotCardComponent, AsyncPipe],
  templateUrl: './recipe-slot.component.html',
  styleUrl: './recipe-slot.component.css',
})
export class RecipeSlotComponent {
  private readonly recipeSelectService = inject(RecipeSelectService);

  public readonly isRecipeSelectOpen$ =
    this.recipeSelectService.recipeSelectOpen$;

  @Input({ required: true }) isEmpty!: boolean;
  @Input({ required: true }) title!: string;
  @Input() imageUrl?: string;

  @Output() removed = new EventEmitter<boolean>();

  remove() {
    this.removed.emit(true);
  }
}
