import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MealSelectComponent } from './meal-select.component';

describe('MealSelectComponent', () => {
  let component: MealSelectComponent;
  let fixture: ComponentFixture<MealSelectComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MealSelectComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(MealSelectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
