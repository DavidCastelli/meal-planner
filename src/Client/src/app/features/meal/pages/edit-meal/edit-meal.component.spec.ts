import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditMealComponent } from './edit-meal.component';

describe('EditMealComponent', () => {
  let component: EditMealComponent;
  let fixture: ComponentFixture<EditMealComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditMealComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(EditMealComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
