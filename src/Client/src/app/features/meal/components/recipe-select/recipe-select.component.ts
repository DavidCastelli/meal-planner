import {
  AfterViewInit,
  Component,
  DestroyRef,
  ElementRef,
  EventEmitter,
  inject,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { RecipeService } from '../../../recipe/recipe.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { GetRecipesDto } from '../../../recipe/models/get/get-recipes-dto.model';
import { RecipeSelectCardComponent } from '../recipe-select-card/recipe-select-card.component';
import { CdkDrag, CdkDropList } from '@angular/cdk/drag-drop';
import { RecipeSelectService } from '../../recipe-select.service';
import { FormControl } from '@angular/forms';
import { map } from 'rxjs';
import { RecipeCardData } from '../../recipe-card-data';

@Component({
  selector: 'app-recipe-select',
  standalone: true,
  imports: [RecipeSelectCardComponent, CdkDropList, CdkDrag],
  templateUrl: './recipe-select.component.html',
  styleUrl: './recipe-select.component.css',
})
export class RecipeSelectComponent implements OnInit, AfterViewInit {
  @ViewChild('recipeSelectBottom')
  recipeSelectBottomRef!: ElementRef<HTMLElement>;

  private readonly recipeService = inject(RecipeService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly recipeSelectService = inject(RecipeSelectService);

  public recipes!: RecipeCardData[];

  @Input({ required: true }) recipeIds!: FormControl<number[]>;

  @Output() viewChanged = new EventEmitter<ElementRef<HTMLElement>>();

  ngOnInit() {
    this.getRecipes();
    this.refreshRecipes();
  }

  ngAfterViewInit() {
    this.viewChanged.emit(this.recipeSelectBottomRef);
  }

  close() {
    this.recipeSelectService.setIsOpen(false);
  }

  private getRecipes() {
    this.recipeService
      .getRecipes()
      .pipe(
        map((recipes: GetRecipesDto[]) => {
          return recipes
            .filter(
              (recipe: GetRecipesDto) =>
                !this.recipeIds.value.includes(recipe.id),
            )
            .map((recipe): RecipeCardData => {
              return {
                id: recipe.id,
                title: recipe.title,
                imageUrl: recipe.image?.imageUrl,
              };
            })
            .sort((recipeA, recipeB) => {
              return recipeA.title.localeCompare(recipeB.title);
            });
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((recipes) => {
        this.recipes = recipes;
      });
  }

  private refreshRecipes() {
    this.recipeSelectService.recipeRemoved$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((recipe) => {
        this.recipes.push(recipe);
        this.recipes.sort((recipeA, recipeB) => {
          return recipeA.title.localeCompare(recipeB.title);
        });
      });
  }
}
