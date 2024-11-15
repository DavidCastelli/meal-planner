import { Component, inject, Input } from '@angular/core';
import { RecipeService } from '../../recipe.service';
import { GetByIdRecipeDto } from '../../models/get/get-by-id-recipe-dto.model';

@Component({
  selector: 'app-edit-recipe',
  standalone: true,
  imports: [],
  templateUrl: './edit-recipe.component.html',
  styleUrl: './edit-recipe.component.css',
})
export class EditRecipeComponent {
  private readonly recipeService = inject(RecipeService);

  @Input() recipe!: GetByIdRecipeDto;
}
