import { Component, DestroyRef, inject } from '@angular/core';
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
import { CreateRecipeRequestDirection } from '../../models/create/create-recipe-request-direction.model';
import { CreateRecipeRequestTip } from '../../models/create/create-recipe-request-tip.model';
import { CreateRecipeRequestIngredient } from '../../models/create/create-recipe-request-ingredient.model';
import { CreateRecipeRequestSubIngredient } from '../../models/create/create-recipe-request-subingredient.model';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NgOptimizedImage } from '@angular/common';
import { ControlErrorComponent } from '../../../../shared/components/control-error/control-error.component';

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
export class CreateRecipeComponent {
  private readonly recipeService = inject(RecipeService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly router = inject(Router);
  private readonly formBuilder = inject(FormBuilder);

  public readonly errorService = inject(ErrorService);

  private directionCount = 1;

  isSubmitting = false;
  previewSrc = '/27002.jpg';
  previewFileName = 'No Image Selected.';

  createRecipeForm = this.formBuilder.nonNullable.group<CreateRecipeForm>({
    title: this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.maxLength(20),
    ]),
    image: this.formBuilder.control<File | null>(null),
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

  get title() {
    return this.createRecipeForm.controls.title;
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
    const direction = this.formBuilder.nonNullable.group({
      number: [this.directionCount],
      description: [
        '',
        { validators: [Validators.required, Validators.maxLength(255)] },
      ],
    });
    this.directionCount += 1;
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
    const subIngredient = this.formBuilder.nonNullable.group({
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
    });
    this.subIngredients.push(subIngredient);
  }

  addSubIngredientName(index: number) {
    const subIngredientName = this.formBuilder.nonNullable.control('', [
      Validators.required,
      Validators.maxLength(20),
    ]);
    this.subIngredients.at(index).addControl('name', subIngredientName);
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
  }

  removeSubIngredientName(index: number) {
    this.subIngredients.at(index).removeControl('name');
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
    }
  }

  submit() {
    this.isSubmitting = true;
    this.errorService.clear();

    if (this.createRecipeForm.invalid) {
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
    const directions = [];
    for (const group of this.directions.controls) {
      const direction: CreateRecipeRequestDirection = {
        Number: group.controls.number.defaultValue,
        Description: group.controls.description.value,
      };
      directions.push(direction);
    }

    const tips = [];
    for (const control of this.tips.controls) {
      const tip: CreateRecipeRequestTip = {
        Description: control.value,
      };
      tips.push(tip);
    }

    const subIngredients = [];
    for (const group1 of this.subIngredients.controls) {
      const ingredients = [];
      for (const group2 of group1.controls.ingredients.controls) {
        const ingredient: CreateRecipeRequestIngredient = {
          Name: group2.controls.name.value,
          Measurement: group2.controls.measurement.value,
        };
        ingredients.push(ingredient);
      }

      const subIngredient: CreateRecipeRequestSubIngredient = {
        Name: group1.controls.name?.value,
        Ingredients: ingredients,
      };
      subIngredients.push(subIngredient);
    }

    return {
      Title: this.title.value,
      Description: this.description?.value,
      Details: {
        PrepTime: this.details.controls.prepTime?.value,
        CookTime: this.details.controls.cookTime?.value,
        Servings: this.details.controls.cookTime?.value,
      },
      Nutrition: {
        Calories: this.nutrition.controls.calories?.value,
        Fat: this.nutrition.controls.fat?.value,
        Carbs: this.nutrition.controls.carbs?.value,
        Protein: this.nutrition.controls.protein?.value,
      },
      Directions: directions,
      Tips: tips,
      SubIngredients: subIngredients,
    };
  }
}