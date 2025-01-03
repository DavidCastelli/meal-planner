import { GetRecipeByIdIngredientDto } from './get-recipe-by-id-ingredient-dto.model';

export interface GetRecipeByIdSubIngredientDto {
  id: number;
  name?: string;
  ingredients: GetRecipeByIdIngredientDto[];
}
