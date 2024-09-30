import { UpdateRecipeRequestIngredient } from './update-recipe-request-ingredient.model';

export interface UpdateRecipeRequestSubIngredient {
  Name?: string;
  Ingredients: UpdateRecipeRequestIngredient[];
}
