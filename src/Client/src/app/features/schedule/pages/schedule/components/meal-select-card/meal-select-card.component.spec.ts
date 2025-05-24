import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MealSelectCardComponent } from './meal-select-card.component';

describe('MealSelectCardComponent', () => {
  let component: MealSelectCardComponent;
  let fixture: ComponentFixture<MealSelectCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MealSelectCardComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(MealSelectCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
