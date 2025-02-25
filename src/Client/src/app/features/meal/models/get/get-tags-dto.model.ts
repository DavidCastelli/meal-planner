import { TagType } from '../../tag-type.enum';

export interface GetTagsDto {
  id: number;
  tagType: TagType;
}
