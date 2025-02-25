import { Component, Input } from '@angular/core';
import { CdkListbox, CdkOption } from '@angular/cdk/listbox';
import { GetTagsDto } from '../../models/get/get-tags-dto.model';
import { TagType } from '../../tag-type.enum';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-tag-list-box',
  standalone: true,
  imports: [CdkListbox, CdkOption, ReactiveFormsModule],
  templateUrl: './tag-list-box.component.html',
  styleUrl: './tag-list-box.component.css',
})
export class TagListBoxComponent {
  protected readonly TagType = TagType;

  @Input({ required: true }) tags!: GetTagsDto[];
  @Input({ required: true }) tagIds!: FormControl<number[]>;

  getSelectedTags(): GetTagsDto[] {
    return this.tags.filter((tag: GetTagsDto) =>
      this.tagIds.value.includes(tag.id),
    );
  }
}
