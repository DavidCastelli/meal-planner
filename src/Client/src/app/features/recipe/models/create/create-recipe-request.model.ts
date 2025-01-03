import { CreateRecipeRequestDetails } from './create-recipe-request-details.model';
import { CreateRecipeRequestNutrition } from './create-recipe-request-nutrition.model';
import { CreateRecipeRequestDirection } from './create-recipe-request-direction.model';
import { CreateRecipeRequestTip } from './create-recipe-request-tip.model';
import { CreateRecipeRequestSubIngredient } from './create-recipe-request-subingredient.model';

export interface CreateRecipeRequest {
  title: string;
  description?: string;
  details: CreateRecipeRequestDetails;
  nutrition: CreateRecipeRequestNutrition;
  directions: CreateRecipeRequestDirection[];
  tips: CreateRecipeRequestTip[];
  subIngredients: CreateRecipeRequestSubIngredient[];
}
