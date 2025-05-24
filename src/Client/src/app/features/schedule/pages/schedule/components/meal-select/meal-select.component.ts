import { Component, DestroyRef, inject, Input, OnInit } from '@angular/core';
import { MealSelectCardComponent } from '../meal-select-card/meal-select-card.component';
import { CdkDrag, CdkDropList } from '@angular/cdk/drag-drop';
import { MealSelectService } from '../../../../meal-select.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MealCardData } from '../../../../meal-card-data';

@Component({
  selector: 'app-meal-select',
  standalone: true,
  imports: [MealSelectCardComponent, CdkDrag, CdkDropList],
  templateUrl: './meal-select.component.html',
  styleUrl: './meal-select.component.css',
})
export class MealSelectComponent implements OnInit {
  private readonly mealSelectService = inject(MealSelectService);
  private readonly destroyRef = inject(DestroyRef);

  @Input({ required: true }) schedulableMeals!: MealCardData[];

  ngOnInit() {
    this.observeAdded();
    this.observeRemoved();
  }

  noReturnPredicate() {
    return false;
  }

  private observeAdded() {
    this.mealSelectService.mealAdded$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((mealAdded: MealCardData) => {
        this.schedulableMeals.push(mealAdded);
        this.schedulableMeals.sort((mealA, mealB) =>
          mealA.title.localeCompare(mealB.title),
        );
      });
  }

  private observeRemoved() {
    this.mealSelectService.mealRemoved$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((mealRemoved: MealCardData) => {
        const index = this.schedulableMeals.findIndex(
          (meal) => meal.id === mealRemoved.id,
        );
        this.schedulableMeals.splice(index, 1);
      });
  }
}
