import { Component, inject, Input } from '@angular/core';
import { RecipeService } from '../../recipe.service';
import { GetByIdRecipeDto } from '../../models/get/get-by-id-recipe-dto.model';

@Component({
  selector: 'app-recipe-details',
  standalone: true,
  imports: [],
  templateUrl: './recipe-details.component.html',
  styleUrl: './recipe-details.component.css',
})
export class RecipeDetailsComponent {
  private readonly recipeService = inject(RecipeService);

  @Input() recipe!: GetByIdRecipeDto;
}
