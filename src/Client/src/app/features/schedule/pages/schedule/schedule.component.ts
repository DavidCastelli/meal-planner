import { Component, DestroyRef, inject, Input, OnInit } from '@angular/core';
import { slideAnimation } from '../../../../shared/animations/slide.animation';
import { SidebarService } from '../../../../core/layout/sidebar.service';
import { AsyncPipe } from '@angular/common';
import { Schedule } from '../../../../shared/enums/schedule.enum';
import { DayOfWeekComponent } from './components/day-of-week/day-of-week.component';
import { MealCardData } from '../../meal-card-data';
import { ScheduleService } from '../../schedule.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MealSelectComponent } from './components/meal-select/meal-select.component';
import { MealSelectService } from '../../meal-select.service';
import { GetMealsDto } from '../../models/get-meals-dto.model';

@Component({
  selector: 'app-schedule',
  standalone: true,
  imports: [AsyncPipe, DayOfWeekComponent, MealSelectComponent],
  animations: [slideAnimation],
  templateUrl: './schedule.component.html',
  styleUrl: './schedule.component.css',
})
export class ScheduleComponent implements OnInit {
  private readonly scheduleService = inject(ScheduleService);
  private readonly sidebarService = inject(SidebarService);
  private readonly mealSelectService = inject(MealSelectService);
  private readonly destroyRef = inject(DestroyRef);

  public readonly isSidebarOpen$ = this.sidebarService.openClose$;
  public readonly days: Schedule[] = [
    Schedule.Monday,
    Schedule.Tuesday,
    Schedule.Wednesday,
    Schedule.Thursday,
    Schedule.Friday,
    Schedule.Saturday,
    Schedule.Sunday,
  ];
  public scheduledMeals: (MealCardData | undefined)[] = [];
  public schedulableMeals: MealCardData[] = [];

  protected readonly Schedule = Schedule;

  @Input() schedule: GetMealsDto[] = [];
  @Input() meals: GetMealsDto[] = [];

  ngOnInit() {
    for (const day of this.days) {
      const meal = this.schedule.find((sm) => sm.schedule === day);
      const data =
        meal === undefined
          ? undefined
          : {
              id: meal.id,
              title: meal.title,
              imageUrl: meal.image?.imageUrl ?? '/27002.jpg',
            };
      this.scheduledMeals.push(data);
    }

    this.schedulableMeals = this.meals
      .filter((mcd) => !this.schedule.map((scd) => scd.id).includes(mcd.id))
      .map((meal) => {
        return {
          id: meal.id,
          title: meal.title,
          imageUrl: meal.image?.imageUrl ?? '/27002.jpg',
        };
      });
  }

  add(data: { day: string; mealCardData: MealCardData }) {
    const id = data.mealCardData.id;
    const day: Schedule = Schedule[data.day as keyof typeof Schedule];

    this.scheduleService
      .addScheduledMeal(id, day)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((success) => {
        if (success) {
          this.mealSelectService.setMealRemoved(data.mealCardData);
          this.scheduledMeals[day - 1] = data.mealCardData;
        }
      });
  }

  clear() {
    this.scheduleService
      .clearScheduledMeals()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((success) => {
        if (success) {
          for (const meal of this.scheduledMeals) {
            if (meal !== undefined) {
              this.mealSelectService.setMealAdded(meal);
            }
          }
          this.scheduledMeals = this.scheduledMeals.map((meal) => {
            if (meal !== undefined) {
              return undefined;
            }

            return meal;
          });
        }
      });
  }

  remove(id: number) {
    this.scheduleService
      .removeScheduledMeal(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((success) => {
        if (success) {
          const index = this.scheduledMeals.findIndex((m) =>
            m === undefined ? false : m.id === id,
          );
          this.mealSelectService.setMealAdded(this.scheduledMeals[index]!);
          this.scheduledMeals[index] = undefined;
        }
      });
  }
}
