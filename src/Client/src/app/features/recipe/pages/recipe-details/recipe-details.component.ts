import { Component, inject, Input } from '@angular/core';
import { RecipeService } from '../../recipe.service';
import { GetRecipeByIdDto } from '../../models/get/get-recipe-by-id-dto.model';

@Component({
  selector: 'app-recipe-details',
  standalone: true,
  imports: [],
  templateUrl: './recipe-details.component.html',
  styleUrl: './recipe-details.component.css',
})
export class RecipeDetailsComponent {
  private readonly recipeService = inject(RecipeService);

  @Input() recipe!: GetRecipeByIdDto;
}
