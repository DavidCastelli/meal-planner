import { ResolveFn } from '@angular/router';
import { inject } from '@angular/core';
import { RecipeService } from '../recipe.service';
import { GetRecipeByIdDto } from '../models/get/get-recipe-by-id-dto.model';
import { map } from 'rxjs';

export const recipeResolver: ResolveFn<GetRecipeByIdDto> = (route) => {
  return inject(RecipeService)
    .getRecipe(route.params['id'])
    .pipe(
      map((recipe: GetRecipeByIdDto) => {
        recipe.directions.sort((directionA, directionB) => {
          if (directionA.number > directionB.number) {
            return 1;
          } else if (directionA.number < directionB.number) {
            return -1;
          } else {
            return 0;
          }
        });

        recipe.tips.sort((tipA, tipB) => {
          if (tipA.id > tipB.id) {
            return 1;
          } else if (tipA.id < tipB.id) {
            return -1;
          } else {
            return 0;
          }
        });

        recipe.subIngredients.sort((subIngredientA, subIngredientB) => {
          if (subIngredientA.id > subIngredientB.id) {
            return 1;
          } else if (subIngredientA.id < subIngredientB.id) {
            return -1;
          } else {
            return 0;
          }
        });

        for (const subIngredient of recipe.subIngredients) {
          subIngredient.ingredients.sort((ingredientA, ingredientB) => {
            if (ingredientA.id > ingredientB.id) {
              return 1;
            } else if (ingredientA.id < ingredientB.id) {
              return -1;
            } else {
              return 0;
            }
          });
        }

        return recipe;
      }),
    );
};
