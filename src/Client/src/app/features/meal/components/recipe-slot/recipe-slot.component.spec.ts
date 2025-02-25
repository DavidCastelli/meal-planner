import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecipeSlotComponent } from './recipe-slot.component';

describe('RecipeSlotComponent', () => {
  let component: RecipeSlotComponent;
  let fixture: ComponentFixture<RecipeSlotComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RecipeSlotComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(RecipeSlotComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
