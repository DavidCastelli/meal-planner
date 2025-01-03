import {
  Component,
  DestroyRef,
  ElementRef,
  HostListener,
  inject,
  Input,
  OnInit,
  ViewChild,
} from '@angular/core';
import { RecipeService } from '../../recipe.service';
import { GetRecipeByIdDto } from '../../models/get/get-recipe-by-id-dto.model';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { ModalService } from '../../../../core/services/modal.service';
import { SidebarService } from '../../../../core/layout/sidebar.service';
import { ErrorService } from '../../../../core/errors/error.service';
import { ImageValidator } from '../../../../shared/validators/image.validator';
import { AsyncPipe, NgOptimizedImage } from '@angular/common';
import { ControlErrorComponent } from '../../../../shared/components/control-error/control-error.component';
import { map, Observable } from 'rxjs';
import { CanComponentDeactivate } from '../../../../core/interfaces/can-component-deactivate';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { UpdateRecipeRequest } from '../../models/update/update-recipe-request.model';
import { FormErrorsComponent } from '../../../../shared/components/form-errors/form-errors.component';
import { slideAnimation } from '../../../../shared/animations/slide.animation';
import { ImageService } from '../../../../shared/services/image.service';

interface RecipeDetailsForm {
  prepTime?: FormControl<number>;
  cookTime?: FormControl<number>;
  servings?: FormControl<number>;
}

interface RecipeNutritionForm {
  calories?: FormControl<number>;
  fat?: FormControl<number>;
  carbs?: FormControl<number>;
  protein?: FormControl<number>;
}

interface SubIngredientForm {
  id: FormControl<number>;
  name?: FormControl<string>;
  ingredients: FormArray<
    FormGroup<{
      id: FormControl<number>;
      name: FormControl<string>;
      measurement: FormControl<string>;
    }>
  >;
}

interface EditRecipeForm {
  title: FormControl<string>;
  image: FormControl<File | null>;
  description?: FormControl<string>;
  details: FormGroup<RecipeDetailsForm>;
  nutrition: FormGroup<RecipeNutritionForm>;
  directions: FormArray<
    FormGroup<{
      id: FormControl<number>;
      number: FormControl<number>;
      description: FormControl<string>;
    }>
  >;
  tips: FormArray<
    FormGroup<{
      id: FormControl<number>;
      description: FormControl<string>;
    }>
  >;
  subIngredients: FormArray<FormGroup<SubIngredientForm>>;
}

@Component({
  selector: 'app-edit-recipe',
  standalone: true,
  imports: [
    AsyncPipe,
    ReactiveFormsModule,
    ControlErrorComponent,
    FormErrorsComponent,
    NgOptimizedImage,
  ],
  animations: [slideAnimation],
  templateUrl: './edit-recipe.component.html',
  styleUrl: './edit-recipe.component.css',
})
export class EditRecipeComponent implements OnInit, CanComponentDeactivate {
  @ViewChild('bottom') formBottomRef?: ElementRef<HTMLElement>;

  private readonly recipeService = inject(RecipeService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly router = inject(Router);
  private readonly formBuilder = inject(FormBuilder);
  private readonly modalService = inject(ModalService);
  private readonly sidebarService = inject(SidebarService);
  private readonly imageService = inject(ImageService);

  public readonly errorService = inject(ErrorService);
  public readonly isSideBarOpen$ = this.sidebarService.openClose$;

  private directionCount = 0;
  private isExitModalOpen = false;

  isSubmitting = false;
  previewSrc = '/27002.jpg';
  previewFileName = 'No Image Selected.';

  @Input() recipe!: GetRecipeByIdDto;

  editRecipeForm = this.formBuilder.nonNullable.group<EditRecipeForm>({
    title: this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.maxLength(20),
    ]),
    image: this.formBuilder.control<File | null>(null, [
      ImageValidator.empty,
      ImageValidator.max,
      ImageValidator.extensions,
    ]),
    details: this.formBuilder.nonNullable.group({}),
    nutrition: this.formBuilder.nonNullable.group({}),
    directions: this.formBuilder.nonNullable.array<FormGroup>([]),
    tips: this.formBuilder.nonNullable.array<FormGroup>([]),
    subIngredients: this.formBuilder.nonNullable.array<FormGroup>([]),
  });

  ngOnInit() {
    const recipeImage = this.recipe.image;
    if (recipeImage) {
      this.imageService
        .getImage(recipeImage.storageFileName)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe((image) => {
          if (image && image.size > 0) {
            const fileReader = new FileReader();
            fileReader.onload = () => {
              this.previewSrc = fileReader.result as string;
            };

            fileReader.readAsDataURL(image);
            this.previewFileName = recipeImage.displayFileName;

            this.editRecipeForm.patchValue({
              image: new File([image], recipeImage.displayFileName, {
                type: 'image/jpeg',
                lastModified: Date.now(),
              }),
            });
          } else {
            console.error('Failed to load recipe image.');
          }
        });
    }

    if (this.recipe.description) {
      this.addDescription();
    }

    const recipeDetails = this.recipe.details;
    if (recipeDetails.prepTime || recipeDetails.prepTime === 0) {
      this.addPrepTime();
    }
    if (recipeDetails.cookTime || recipeDetails.cookTime === 0) {
      this.addCookTime();
    }
    if (recipeDetails.servings || recipeDetails.servings === 0) {
      this.addServings();
    }

    const recipeNutrition = this.recipe.nutrition;
    if (recipeNutrition.calories || recipeNutrition.calories === 0) {
      this.addCalories();
    }
    if (recipeNutrition.fat || recipeNutrition.fat === 0) {
      this.addFat();
    }
    if (recipeNutrition.carbs || recipeNutrition.carbs === 0) {
      this.addCarbs();
    }
    if (recipeNutrition.protein || recipeNutrition.protein === 0) {
      this.addProtein();
    }

    this.recipe.directions.forEach(() => this.addDirection());

    this.recipe.tips.forEach(() => this.addTip());

    for (let i = 0; i < this.recipe.subIngredients.length; i++) {
      this.addSubIngredient();

      for (
        let j = 0;
        j < this.recipe.subIngredients[i].ingredients.length - 1;
        j++
      ) {
        this.addIngredient(i);
      }
    }

    this.editRecipeForm.patchValue({
      title: this.recipe.title,
      description: this.recipe.description,
      details: this.recipe.details,
      nutrition: this.recipe.nutrition,
      directions: this.recipe.directions,
      tips: this.recipe.tips,
      subIngredients: this.recipe.subIngredients,
    });

    // Needed because the add control functions above mark the form as dirty.
    this.editRecipeForm.markAsPristine();
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
        'Are you sure you want to exit? All changes for the current recipe will be lost.';

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
    return this.editRecipeForm.controls.image;
  }

  get description() {
    return this.editRecipeForm.controls.description;
  }

  get details() {
    return this.editRecipeForm.controls.details;
  }

  get nutrition() {
    return this.editRecipeForm.controls.nutrition;
  }

  get directions() {
    return this.editRecipeForm.controls.directions;
  }

  get tips() {
    return this.editRecipeForm.controls.tips;
  }

  get subIngredients() {
    return this.editRecipeForm.controls.subIngredients;
  }

  addDescription() {
    const description = this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.maxLength(255),
    ]);
    this.editRecipeForm.addControl('description', description);
    this.editRecipeForm.markAsDirty();
  }

  addPrepTime() {
    const prepTime = this.formBuilder.nonNullable.control(0, [
      Validators.required,
      Validators.min(0),
      Validators.max(2147483647),
      Validators.pattern('[0-9]*'),
    ]);
    this.details.addControl('prepTime', prepTime);
    this.details.markAsDirty();
  }

  addCookTime() {
    const cookTime = this.formBuilder.nonNullable.control(0, [
      Validators.required,
      Validators.min(0),
      Validators.max(2147483647),
      Validators.pattern('[0-9]*'),
    ]);
    this.details.addControl('cookTime', cookTime);
    this.details.markAsDirty();
  }

  addServings() {
    const servings = this.formBuilder.nonNullable.control(0, [
      Validators.required,
      Validators.min(0),
      Validators.max(2147483647),
      Validators.pattern('[0-9]*'),
    ]);
    this.details.addControl('servings', servings);
    this.details.markAsDirty();
  }

  addCalories() {
    const calories = this.formBuilder.nonNullable.control(0, [
      Validators.required,
      Validators.min(0),
      Validators.max(2147483647),
      Validators.pattern('[0-9]*'),
    ]);
    this.nutrition.addControl('calories', calories);
    this.nutrition.markAsDirty();
  }

  addFat() {
    const fat = this.formBuilder.nonNullable.control(0, [
      Validators.required,
      Validators.min(0),
      Validators.max(2147483647),
      Validators.pattern('[0-9]*'),
    ]);
    this.nutrition.addControl('fat', fat);
    this.nutrition.markAsDirty();
  }

  addCarbs() {
    const carbs = this.formBuilder.nonNullable.control(0, [
      Validators.required,
      Validators.min(0),
      Validators.max(2147483647),
      Validators.pattern('[0-9]*'),
    ]);
    this.nutrition.addControl('carbs', carbs);
    this.nutrition.markAsDirty();
  }

  addProtein() {
    const protein = this.formBuilder.nonNullable.control(0, [
      Validators.required,
      Validators.min(0),
      Validators.max(2147483647),
      Validators.pattern('[0-9]*'),
    ]);
    this.nutrition.addControl('protein', protein);
    this.nutrition.markAsDirty();
  }

  addDirection() {
    this.directionCount += 1;
    const direction = this.formBuilder.nonNullable.group({
      id: [0],
      number: [this.directionCount],
      description: [
        '',
        { validators: [Validators.required, Validators.maxLength(255)] },
      ],
    });
    this.directions.push(direction);
    this.directions.markAsDirty();
  }

  addTip() {
    const tip = this.formBuilder.nonNullable.group({
      id: this.formBuilder.nonNullable.control(0),
      description: this.formBuilder.nonNullable.control('', [
        Validators.required,
        Validators.maxLength(255),
      ]),
    });
    this.tips.push(tip);
    this.tips.markAsDirty();
  }

  addSubIngredient() {
    if (this.subIngredients.length === 1) {
      const subIngredientName = this.formBuilder.nonNullable.control('', [
        Validators.required,
        Validators.maxLength(20),
      ]);
      this.subIngredients.at(0).addControl('name', subIngredientName);
    }

    const subIngredient = this.formBuilder.nonNullable.group<SubIngredientForm>(
      {
        id: this.formBuilder.nonNullable.control(0),
        name:
          this.subIngredients.length === 0
            ? undefined
            : this.formBuilder.nonNullable.control('', [
                Validators.required,
                Validators.maxLength(20),
              ]),
        ingredients: this.formBuilder.nonNullable.array([
          this.formBuilder.nonNullable.group({
            id: [0],
            name: [
              '',
              { validators: [Validators.required, Validators.maxLength(20)] },
            ],
            measurement: [
              '',
              { validators: [Validators.required, Validators.maxLength(20)] },
            ],
          }),
        ]),
      },
    );
    this.subIngredients.push(subIngredient);
    this.subIngredients.markAsDirty();
  }

  addIngredient(index: number) {
    const ingredient = this.formBuilder.nonNullable.group({
      id: [0],
      name: [
        '',
        { validators: [Validators.required, Validators.maxLength(20)] },
      ],
      measurement: [
        '',
        { validators: [Validators.required, Validators.maxLength(20)] },
      ],
    });
    const ingredients = this.subIngredients.at(index).controls.ingredients;
    ingredients.push(ingredient);
    ingredients.markAsDirty();
  }

  removeImage() {
    this.previewSrc = '/27002.jpg';
    this.previewFileName = 'No Image Selected.';
    this.editRecipeForm.patchValue({ image: null });
    this.image.markAsDirty();
  }

  removeDescription() {
    this.editRecipeForm.removeControl('description');
    this.editRecipeForm.markAsDirty();
  }

  removePrepTime() {
    this.details.removeControl('prepTime');
    this.details.markAsDirty();
  }

  removeCookTime() {
    this.details.removeControl('cookTime');
    this.details.markAsDirty();
  }

  removeServings() {
    this.details.removeControl('servings');
    this.details.markAsDirty();
  }

  removeCalories() {
    this.nutrition.removeControl('calories');
    this.nutrition.markAsDirty();
  }

  removeFat() {
    this.nutrition.removeControl('fat');
    this.nutrition.markAsDirty();
  }

  removeCarbs() {
    this.nutrition.removeControl('carbs');
    this.nutrition.markAsDirty();
  }

  removeProtein() {
    this.nutrition.removeControl('protein');
    this.nutrition.markAsDirty();
  }

  removeDirection(index: number) {
    this.directionCount -= 1;
    this.directions.removeAt(index);
    for (let i = index; i < this.directions.length; i++) {
      const currentDirectionNumber = this.directions.at(i).controls.number;
      currentDirectionNumber.setValue(currentDirectionNumber.value - 1);
    }
    this.directions.markAsDirty();
  }

  removeTip(index: number) {
    this.tips.removeAt(index);
    this.tips.markAsDirty();
  }

  removeSubIngredient(index: number) {
    this.subIngredients.removeAt(index);
    if (this.subIngredients.length < 2) {
      this.subIngredients.at(0).removeControl('name');
    }
    this.subIngredients.markAsDirty();
  }

  removeIngredient(subIngredientIndex: number, ingredientIndex: number) {
    const ingredients =
      this.subIngredients.at(subIngredientIndex).controls.ingredients;
    ingredients.removeAt(ingredientIndex);
    ingredients.markAsDirty();
  }

  onImagePicked(event: Event) {
    const fileList = (event.target as HTMLInputElement).files;

    if (fileList && fileList[0]) {
      const image = fileList[0];

      const fileReader = new FileReader();
      fileReader.onload = () => (this.previewSrc = fileReader.result as string);

      fileReader.readAsDataURL(image);
      this.previewFileName = image.name;

      this.editRecipeForm.patchValue({ image: image });
      this.image.updateValueAndValidity();
      this.image.markAsDirty();
    }
  }

  submit() {
    this.isSubmitting = true;
    this.errorService.clear();

    if (this.editRecipeForm.invalid) {
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
    this.recipeService
      .updateRecipe(this.recipe.id, request, image)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((success) => {
        if (success) {
          void this.router.navigate(['/manage/recipes/']);
        } else {
          this.isSubmitting = false;
        }
      });
  }

  private fromForm(): UpdateRecipeRequest {
    const values = this.editRecipeForm.getRawValue();

    return {
      id: this.recipe.id,
      title: values.title,
      description: values.description,
      details: values.details,
      nutrition: values.nutrition,
      directions: values.directions,
      tips: values.tips,
      subIngredients: values.subIngredients,
    };
  }

  private isFormTouchedOrDirty(): boolean {
    return this.editRecipeForm.touched || this.editRecipeForm.dirty;
  }
}
