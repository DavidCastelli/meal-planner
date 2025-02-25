import { ResolveFn } from '@angular/router';
import { GetTagsDto } from '../models/get/get-tags-dto.model';
import { inject } from '@angular/core';
import { TagService } from '../tag.service';
import { map } from 'rxjs';

export const tagsResolver: ResolveFn<GetTagsDto[]> = () => {
  return inject(TagService)
    .getTags()
    .pipe(
      map((tags: GetTagsDto[]) => {
        return tags.sort((tagA, tagB) => {
          if (tagA.id > tagB.id) {
            return 1;
          } else if (tagA.id < tagB.id) {
            return -1;
          } else {
            return 0;
          }
        });
      }),
    );
};
