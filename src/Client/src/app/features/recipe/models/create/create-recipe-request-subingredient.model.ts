import {CreateRecipeRequestIngredient} from "./create-recipe-request-ingredient.model";

export interface CreateRecipeRequestSubIngredient {
  Name?: string;
  Ingredients: CreateRecipeRequestIngredient[]
}
