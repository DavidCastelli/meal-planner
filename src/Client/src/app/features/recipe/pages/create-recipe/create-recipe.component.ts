import { Component, DestroyRef, HostListener, inject } from '@angular/core';
import { ErrorService } from '../../../../core/errors/error.service';
import { Router } from '@angular/router';
import { RecipeService } from '../../recipe.service';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CreateRecipeRequest } from '../../models/create/create-recipe-request.model';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NgOptimizedImage } from '@angular/common';
import { ControlErrorComponent } from '../../../../shared/components/control-error/control-error.component';
import { ImageValidator } from '../../../../shared/validators/image.validator';
import { CanComponentDeactivate } from '../../../../shared/interfaces/can-component-deactivate';
import { ModalService } from '../../../../shared/services/modal.service';
import { map, Observable } from 'rxjs';

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
  name?: FormControl<string>;
  ingredients: FormArray<
    FormGroup<{
      name: FormControl<string>;
      measurement: FormControl<string>;
    }>
  >;
}

interface CreateRecipeForm {
  title: FormControl<string>;
  image: FormControl<File | null>;
  description?: FormControl<string>;
  details: FormGroup<RecipeDetailsForm>;
  nutrition: FormGroup<RecipeNutritionForm>;
  directions: FormArray<
    FormGroup<{
      number: FormControl<number>;
      description: FormControl<string>;
    }>
  >;
  tips: FormArray<FormControl<string>>;
  subIngredients: FormArray<FormGroup<SubIngredientForm>>;
}

@Component({
  selector: 'app-create-recipe',
  standalone: true,
  imports: [ReactiveFormsModule, NgOptimizedImage, ControlErrorComponent],
  templateUrl: './create-recipe.component.html',
  styleUrl: './create-recipe.component.css',
})
export class CreateRecipeComponent implements CanComponentDeactivate {
  private readonly recipeService = inject(RecipeService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly router = inject(Router);
  private readonly formBuilder = inject(FormBuilder);
  private readonly modalService = inject(ModalService);

  public readonly errorService = inject(ErrorService);

  private directionCount = 1;
  private isExitModalOpen = false;

  isSubmitting = false;
  previewSrc = '/27002.jpg';
  previewFileName = 'No Image Selected.';

  createRecipeForm = this.formBuilder.nonNullable.group<CreateRecipeForm>({
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
    directions: this.formBuilder.nonNullable.array([
      this.formBuilder.nonNullable.group({
        number: [this.directionCount],
        description: [
          '',
          { validators: [Validators.required, Validators.maxLength(255)] },
        ],
      }),
    ]),
    tips: this.formBuilder.nonNullable.array<FormControl<string>>([]),
    subIngredients: this.formBuilder.nonNullable.array([
      this.formBuilder.nonNullable.group({
        ingredients: this.formBuilder.nonNullable.array([
          this.formBuilder.nonNullable.group({
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
      }),
    ]),
  });

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
        setTimeout(() =>
        {
          this.isExitModalOpen = false;
        }, 100);
      }, 1);

      event.preventDefault();
    }
  }

  canDeactivate(): boolean | Observable<boolean> {
    if (this.isExitModalOpen) {
      return false;
    }

    if (this.isFormTouchedOrDirty()) {
      const title = 'Exit';
      const message =
        'Are you sure you want to exit? All data for the current recipe will be lost.';

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
    return this.createRecipeForm.controls.image;
  }

  get description() {
    return this.createRecipeForm.controls.description;
  }

  get details() {
    return this.createRecipeForm.controls.details;
  }

  get nutrition() {
    return this.createRecipeForm.controls.nutrition;
  }

  get directions() {
    return this.createRecipeForm.controls.directions;
  }

  get tips() {
    return this.createRecipeForm.controls.tips;
  }

  get subIngredients() {
    return this.createRecipeForm.controls.subIngredients;
  }

  addDescription() {
    const description = this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.maxLength(255),
    ]);
    this.createRecipeForm.addControl('description', description);
  }

  addPrepTime() {
    const prepTime = this.formBuilder.nonNullable.control(0, [
      Validators.required,
      Validators.min(0),
      Validators.max(2147483647),
      Validators.pattern('[0-9]*'),
    ]);
    this.details.addControl('prepTime', prepTime);
  }

  addCookTime() {
    const cookTime = this.formBuilder.nonNullable.control(0, [
      Validators.required,
      Validators.min(0),
      Validators.max(2147483647),
      Validators.pattern('[0-9]*'),
    ]);
    this.details.addControl('cookTime', cookTime);
  }

  addServings() {
    const servings = this.formBuilder.nonNullable.control(0, [
      Validators.required,
      Validators.min(0),
      Validators.max(2147483647),
      Validators.pattern('[0-9]*'),
    ]);
    this.details.addControl('servings', servings);
  }

  addCalories() {
    const calories = this.formBuilder.nonNullable.control(0, [
      Validators.required,
      Validators.min(0),
      Validators.max(2147483647),
      Validators.pattern('[0-9]*'),
    ]);
    this.nutrition.addControl('calories', calories);
  }

  addFat() {
    const fat = this.formBuilder.nonNullable.control(0, [
      Validators.required,
      Validators.min(0),
      Validators.max(2147483647),
      Validators.pattern('[0-9]*'),
    ]);
    this.nutrition.addControl('fat', fat);
  }

  addCarbs() {
    const carbs = this.formBuilder.nonNullable.control(0, [
      Validators.required,
      Validators.min(0),
      Validators.max(2147483647),
      Validators.pattern('[0-9]*'),
    ]);
    this.nutrition.addControl('carbs', carbs);
  }

  addProtein() {
    const protein = this.formBuilder.nonNullable.control(0, [
      Validators.required,
      Validators.min(0),
      Validators.max(2147483647),
      Validators.pattern('[0-9]*'),
    ]);
    this.nutrition.addControl('protein', protein);
  }

  addDirection() {
    this.directionCount += 1;
    const direction = this.formBuilder.nonNullable.group({
      number: [this.directionCount],
      description: [
        '',
        { validators: [Validators.required, Validators.maxLength(255)] },
      ],
    });
    this.directions.push(direction);
  }

  addTip() {
    const tip = this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.maxLength(255),
    ]);
    this.tips.push(tip);
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
        name: this.formBuilder.nonNullable.control('', [
          Validators.required,
          Validators.maxLength(20),
        ]),
        ingredients: this.formBuilder.nonNullable.array([
          this.formBuilder.nonNullable.group({
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
  }

  addIngredient(index: number) {
    const ingredient = this.formBuilder.nonNullable.group({
      name: [
        '',
        { validators: [Validators.required, Validators.maxLength(20)] },
      ],
      measurement: [
        '',
        { validators: [Validators.required, Validators.maxLength(20)] },
      ],
    });
    this.subIngredients.at(index).controls.ingredients.push(ingredient);
  }

  removeImage() {
    this.previewSrc = '/27002.jpg';
    this.previewFileName = 'No Image Selected.';
    this.createRecipeForm.patchValue({ image: null });
  }

  removeDescription() {
    this.createRecipeForm.removeControl('description');
  }

  removePrepTime() {
    this.details.removeControl('prepTime');
  }

  removeCookTime() {
    this.details.removeControl('cookTime');
  }

  removeServings() {
    this.details.removeControl('servings');
  }

  removeCalories() {
    this.nutrition.removeControl('calories');
  }

  removeFat() {
    this.nutrition.removeControl('fat');
  }

  removeCarbs() {
    this.nutrition.removeControl('carbs');
  }

  removeProtein() {
    this.nutrition.removeControl('protein');
  }

  removeDirection(index: number) {
    this.directionCount -= 1;
    this.directions.removeAt(index);
  }

  removeTip(index: number) {
    this.tips.removeAt(index);
  }

  removeSubIngredient(index: number) {
    this.subIngredients.removeAt(index);
    if (this.subIngredients.length < 2) {
      this.subIngredients.at(0).removeControl('name');
    }
  }

  removeIngredient(subIngredientIndex: number, ingredientIndex: number) {
    this.subIngredients
      .at(subIngredientIndex)
      .controls.ingredients.removeAt(ingredientIndex);
  }

  onImagePicked(event: Event) {
    const fileList = (event.target as HTMLInputElement).files;

    if (fileList && fileList[0]) {
      const image = fileList[0];

      const fileReader = new FileReader();
      fileReader.onload = () => (this.previewSrc = fileReader.result as string);

      fileReader.readAsDataURL(image);
      this.previewFileName = image.name;

      this.createRecipeForm.patchValue({ image: image });
      this.image.updateValueAndValidity();
    }
  }

  submit() {
    this.isSubmitting = true;
    this.errorService.clear();

    if (this.createRecipeForm.invalid) {
      this.errorService.addMessage('Please fix all validation errors.');
      this.isSubmitting = false;
      return;
    }

    const request = this.fromForm();
    const image = this.image.value;
    this.recipeService
      .CreateRecipe(request, image)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (value) => {
          if (value) {
            void this.router.navigate(['/manage/recipes/']);
          } else {
            this.isSubmitting = false;
          }
        },
      });
  }

  private fromForm(): CreateRecipeRequest {
    const values = this.createRecipeForm.getRawValue();

    return {
      title: values.title,
      description: values.description,
      details: values.details,
      nutrition: values.nutrition,
      directions: values.directions,
      tips: values.tips.map((description) => {
        return { description: description };
      }),
      subIngredients: values.subIngredients,
    };
  }

  private isFormTouchedOrDirty(): boolean {
    return this.createRecipeForm.touched || this.createRecipeForm.dirty;
  }
}
