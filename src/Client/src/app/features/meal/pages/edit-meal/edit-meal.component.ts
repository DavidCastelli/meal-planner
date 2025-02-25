import {
  Component,
  DestroyRef,
  ElementRef,
  HostListener,
  inject,
  Input,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import {
  FormBuilder,
  FormControl,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { slideAnimation } from '../../../../shared/animations/slide.animation';
import { animate, style, transition, trigger } from '@angular/animations';
import { MealService } from '../../meal.service';
import { Router } from '@angular/router';
import { ModalService } from '../../../../core/services/modal.service';
import { SidebarService } from '../../../../core/layout/sidebar.service';
import { RecipeSelectService } from '../../recipe-select.service';
import { ErrorService } from '../../../../core/errors/error.service';
import { GetMealByIdDto } from '../../models/get/get-meal-by-id-dto.model';
import { ImageValidator } from '../../../../shared/validators/image.validator';
import { CanComponentDeactivate } from '../../../../core/interfaces/can-component-deactivate';
import { map, Observable } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { UpdateMealRequest } from '../../models/update/update-meal-request.model';
import { AsyncPipe, NgOptimizedImage } from '@angular/common';
import { ControlErrorComponent } from '../../../../shared/components/control-error/control-error.component';
import { FormErrorsComponent } from '../../../../shared/components/form-errors/form-errors.component';
import { RecipeSelectComponent } from '../../components/recipe-select/recipe-select.component';
import { RecipeSlotsComponent } from '../../components/recipe-slots/recipe-slots.component';
import { TagListBoxComponent } from '../../components/tag-list-box/tag-list-box.component';
import { ImageService } from '../../../../shared/services/image.service';
import { GetTagsDto } from '../../models/get/get-tags-dto.model';

interface EditMealForm {
  title: FormControl<string>;
  image: FormControl<File | null>;
  tagIds: FormControl<number[]>;
  recipeIds: FormControl<number[]>;
}

@Component({
  selector: 'app-edit-meal',
  standalone: true,
  imports: [
    AsyncPipe,
    ControlErrorComponent,
    FormErrorsComponent,
    NgOptimizedImage,
    ReactiveFormsModule,
    RecipeSelectComponent,
    RecipeSlotsComponent,
    TagListBoxComponent,
  ],
  animations: [
    slideAnimation,
    trigger('upDown', [
      transition(':enter', [
        style({ transform: 'translateY(100%)' }),
        animate('600ms 100ms ease-in'),
      ]),
      transition(':leave', [
        animate(
          '600ms 100ms ease-in',
          style({ transform: 'translateY(100%)' }),
        ),
      ]),
    ]),
  ],
  templateUrl: './edit-meal.component.html',
  styleUrl: './edit-meal.component.css',
})
export class EditMealComponent
  implements OnInit, OnDestroy, CanComponentDeactivate
{
  @ViewChild('bottom') formBottomRef?: ElementRef<HTMLElement>;

  private readonly mealService = inject(MealService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly router = inject(Router);
  private readonly formBuilder = inject(FormBuilder);
  private readonly modalService = inject(ModalService);
  private readonly sidebarService = inject(SidebarService);
  private readonly recipeSelectService = inject(RecipeSelectService);
  private readonly imageService = inject(ImageService);

  public readonly errorService = inject(ErrorService);
  public readonly isSideBarOpen$ = this.sidebarService.openClose$;
  public readonly isRecipeSelectOpen$ =
    this.recipeSelectService.recipeSelectOpen$;

  private isExitModalOpen = false;

  isSubmitting = false;
  previewSrc = '27002.jpg';
  previewFileName = 'No Image Selected';

  @Input() meal!: GetMealByIdDto;
  @Input() tags!: GetTagsDto[];

  editMealForm = this.formBuilder.nonNullable.group<EditMealForm>({
    title: this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.maxLength(20),
    ]),
    image: this.formBuilder.nonNullable.control(null, [
      ImageValidator.empty,
      ImageValidator.max,
      ImageValidator.extensions,
    ]),
    tagIds: this.formBuilder.nonNullable.control<number[]>([]),
    recipeIds: this.formBuilder.nonNullable.control<number[]>(
      [],
      [Validators.required],
    ),
  });

  ngOnInit() {
    const mealImage = this.meal.image;
    if (mealImage) {
      this.imageService
        .getImage(mealImage.storageFileName)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe((image) => {
          if (image && image.size > 0) {
            const fileReader = new FileReader();
            fileReader.onload = () => {
              this.previewSrc = fileReader.result as string;
            };

            fileReader.readAsDataURL(image);
            this.previewFileName = mealImage.displayFileName;

            this.editMealForm.patchValue({
              image: new File([image], mealImage.displayFileName, {
                type: 'image/jpeg',
                lastModified: Date.now(),
              }),
            });
          } else {
            console.error('Failed to load meal image.');
          }
        });
    }

    this.editMealForm.patchValue({
      title: this.meal.title,
      tagIds: this.meal.tags.map((tag) => tag.id),
      recipeIds: this.meal.recipes.map((recipe) => recipe.id),
    });
  }

  ngOnDestroy() {
    this.recipeSelectService.setIsOpen(false);
  }

  @HostListener('window:beforeunload', ['$event'])
  onBeforeUnload(event: BeforeUnloadEvent): void {
    if (this.isFormTouchedOrDirty() && !this.isExitModalOpen) {
      // This flag is needed to fix a bug where the user can press the browser back button while the onbeforeload confirm dialog is present.
      // This causes multiple modals to be opened and stack on top of each other in the case the user decides to stay on the page.
      this.isExitModalOpen = true;

      // Needed as a workaround for determining when the onbeforeload dialog ends in the case a user decides to stay.
      // The first timeout triggers immediately before event.preventDefault() is called which opens the dialog and
      // pauses the thread. The inner timeout is then queued and triggers once the dialog closes.
      setTimeout(() => {
        setTimeout(() => {
          this.isExitModalOpen = false;
        }, 100);
      }, 1);

      event.preventDefault();
    }
  }

  canDeactivate(): boolean | Observable<boolean> {
    if (this.isSubmitting) {
      return true;
    }

    if (this.isExitModalOpen) {
      return false;
    }

    if (this.isFormTouchedOrDirty()) {
      const title = 'Exit';
      const message =
        'Are you sure you want to exit? All changes for the current meal will be lost.';

      // Flag used to prevent onbeforeload confirm dialog from stacking on top of the exit modal.
      // If an exit modal is already open and a user refreshes then the page will reload without an additional dialog.
      this.isExitModalOpen = true;

      return this.modalService.openConfirmationModal(title, message).pipe(
        map((result) => {
          this.isExitModalOpen = false;
          if (result === undefined) {
            return false;
          }
          return result;
        }),
      );
    }

    return true;
  }

  get image() {
    return this.editMealForm.controls.image;
  }

  get tagIds() {
    return this.editMealForm.controls.tagIds;
  }

  get recipeIds() {
    return this.editMealForm.controls.recipeIds;
  }

  removeImage() {
    this.previewSrc = '27002.jpg';
    this.previewFileName = 'No Image Selected';
    this.editMealForm.patchValue({ image: null });
    this.image.markAsDirty();
  }

  onImagePicked(event: Event) {
    const fileList = (event.target as HTMLInputElement).files;

    if (fileList && fileList[0]) {
      const image = fileList[0];

      const fileReader = new FileReader();
      fileReader.onload = () => (this.previewSrc = fileReader.result as string);

      fileReader.readAsDataURL(image);
      this.previewFileName = image.name;

      this.editMealForm.patchValue({ image: image });
      this.image.updateValueAndValidity();
      this.image.markAsDirty();
    }
  }

  recipeSelectAnimationStart() {
    this.recipeSelectService.setIsAnimationDone(false);
  }

  recipeSelectAnimationDone() {
    this.recipeSelectService.setIsAnimationDone(true);
  }

  scrollToRecipeSelected(recipeSelect: ElementRef<HTMLElement>) {
    // Timeout is needed so that scrolling happens after the recipe select is rendered to the DOM.
    // Otherwise, the page will not scroll properly to the component.
    setTimeout(() => {
      recipeSelect.nativeElement.scrollIntoView({
        behavior: 'smooth',
      });
    }, 1000);
  }

  submit() {
    this.isSubmitting = true;
    this.errorService.clear();

    if (this.editMealForm.invalid) {
      this.errorService.addMessage('Please fix all validation errors.');
      // Timeout is needed so that scrolling happens after the error message is rendered to the DOM.
      // Otherwise, the page will not scroll properly to the bottom.
      setTimeout(() => {
        this.formBottomRef?.nativeElement.scrollIntoView({
          behavior: 'smooth',
        });
      }, 0);
      this.isSubmitting = false;
      return;
    }

    const request = this.fromForm();
    const image = this.image.value;

    this.mealService
      .updateMeal(this.meal.id, request, image)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((success) => {
        if (success) {
          void this.router.navigate(['/manage/meals/']);
        } else {
          this.isSubmitting = false;
        }
      });
  }

  private fromForm(): UpdateMealRequest {
    const values = this.editMealForm.getRawValue();
    return {
      id: this.meal.id,
      title: values.title,
      tagIds: values.tagIds,
      recipeIds: values.recipeIds,
    };
  }

  private isFormTouchedOrDirty(): boolean {
    return this.editMealForm.touched || this.editMealForm.dirty;
  }
}
