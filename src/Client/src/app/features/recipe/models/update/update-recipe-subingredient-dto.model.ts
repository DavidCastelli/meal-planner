import { UpdateRecipeIngredientDto } from './update-recipe-ingredient-dto.model';

export interface UpdateRecipeSubIngredientDto {
  Name?: string;
  Ingredients: UpdateRecipeIngredientDto[];
}
