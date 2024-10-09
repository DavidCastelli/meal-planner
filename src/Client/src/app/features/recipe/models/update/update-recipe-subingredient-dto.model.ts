import { UpdateRecipeIngredientDto } from './update-recipe-ingredient-dto.model';

export interface UpdateRecipeSubIngredientDto {
  name?: string;
  ingredients: UpdateRecipeIngredientDto[];
}
