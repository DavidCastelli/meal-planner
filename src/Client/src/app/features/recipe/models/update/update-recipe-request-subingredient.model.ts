import { UpdateRecipeRequestIngredient } from './update-recipe-request-ingredient.model';

export interface UpdateRecipeRequestSubIngredient {
  id: number;
  name?: string;
  ingredients: UpdateRecipeRequestIngredient[];
}
