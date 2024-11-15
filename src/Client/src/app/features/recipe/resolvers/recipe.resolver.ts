import { ResolveFn } from '@angular/router';
import { inject } from '@angular/core';
import { RecipeService } from '../recipe.service';
import { GetByIdRecipeDto } from '../models/get/get-by-id-recipe-dto.model';

export const recipeResolver: ResolveFn<GetByIdRecipeDto> = (route) => {
  return inject(RecipeService).getRecipe(route.params['id']);
};
