import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateShoppingListItemComponent } from './create-shopping-list-item.component';

describe('CreateShoppingListItemComponent', () => {
  let component: CreateShoppingListItemComponent;
  let fixture: ComponentFixture<CreateShoppingListItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateShoppingListItemComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateShoppingListItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
