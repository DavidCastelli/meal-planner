import { TagType } from '../../tag-type.enum';

export interface GetMealByIdTagDto {
  id: number;
  tagType: TagType;
}
