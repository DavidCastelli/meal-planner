import { UpdateRecipeRequestIngredient } from './update-recipe-request-ingredient.model';

export interface UpdateRecipeRequestSubIngredient {
  name?: string;
  ingredients: UpdateRecipeRequestIngredient[];
}