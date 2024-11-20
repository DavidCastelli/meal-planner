import { ResolveFn } from '@angular/router';
import { GetRecipesDto } from '../models/get/get-recipes-dto.model';
import { inject } from '@angular/core';
import { RecipeService } from '../recipe.service';
import { map } from 'rxjs';

export const recipesResolver: ResolveFn<GetRecipesDto[]> = () => {
  return inject(RecipeService)
    .getRecipes()
    .pipe(
      map((recipes: GetRecipesDto[]) => {
        return recipes.sort((recipeA, recipeB) => {
          if (recipeA > recipeB) {
            return 1;
          } else if (recipeA < recipeB) {
            return -1;
          } else {
            return 0;
          }
        });
      }),
    );
};
