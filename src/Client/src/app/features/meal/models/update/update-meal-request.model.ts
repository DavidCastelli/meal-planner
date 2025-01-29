export interface UpdateMealRequest {
  id: number;
  title: string;
  tagIds: number[];
  recipeIds: number[];
}
