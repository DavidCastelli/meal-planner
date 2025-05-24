import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScheduledMealPlaceholderComponent } from './scheduled-meal-placeholder.component';

describe('ScheduledMealPlaceholderComponent', () => {
  let component: ScheduledMealPlaceholderComponent;
  let fixture: ComponentFixture<ScheduledMealPlaceholderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ScheduledMealPlaceholderComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ScheduledMealPlaceholderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
