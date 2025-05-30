export interface GetShoppingItemsDto {
  id: number;
  name: string;
  measurement?: string;
  price?: number;
  quantity?: number;
  isChecked: boolean;
  isLocked: boolean;
  isGenerated: boolean;
}
