import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecipeSelectComponent } from './recipe-select.component';

describe('RecipeSelectComponent', () => {
  let component: RecipeSelectComponent;
  let fixture: ComponentFixture<RecipeSelectComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RecipeSelectComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(RecipeSelectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
