import { CreateRecipeRequestRecipeDetails } from './create-recipe-request-recipe-details.model';
import { CreateRecipeRequestRecipeNutrition } from './create-recipe-request-recipe-nutrition.model';
import { CreateRecipeRequestDirection } from './create-recipe-request-direction.model';
import { CreateRecipeRequestTip } from './create-recipe-request-tip.model';
import { CreateRecipeRequestSubIngredient } from './create-recipe-request-subingredient.model';

export interface CreateRecipeRequest {
  title: string;
  description?: string;
  details: CreateRecipeRequestRecipeDetails;
  nutrition: CreateRecipeRequestRecipeNutrition;
  directions: CreateRecipeRequestDirection[];
  tips: CreateRecipeRequestTip[];
  subIngredients: CreateRecipeRequestSubIngredient[];
}
