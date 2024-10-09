import { CreateRecipeRequestIngredient } from './create-recipe-request-ingredient.model';

export interface CreateRecipeRequestSubIngredient {
  name?: string;
  ingredients: CreateRecipeRequestIngredient[];
}
