import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TagListBoxComponent } from './tag-list-box.component';

describe('TagListBoxComponent', () => {
  let component: TagListBoxComponent;
  let fixture: ComponentFixture<TagListBoxComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TagListBoxComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TagListBoxComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
