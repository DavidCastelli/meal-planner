import { ResolveFn } from '@angular/router';
import { GetRecipesDto } from '../models/get/get-recipes-dto.model';
import { inject } from '@angular/core';
import { RecipeService } from '../recipe.service';

export const recipesResolver: ResolveFn<GetRecipesDto[]> = () => {
  return inject(RecipeService).getRecipes();
};
