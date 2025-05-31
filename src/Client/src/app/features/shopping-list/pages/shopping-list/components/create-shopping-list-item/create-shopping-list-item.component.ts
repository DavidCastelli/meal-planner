import {
  Component,
  DestroyRef,
  EventEmitter,
  inject,
  Output,
} from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
} from '@angular/forms';
import { CreateShoppingItemRequest } from '../../../../models/create-shopping-item-request.model';
import { ShoppingListService } from '../../../../shopping-list.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subscription } from 'rxjs';

interface CreateShoppingItemForm {
  name: FormControl<string>;
  measurement?: FormControl<string>;
  price?: FormControl<number>;
  quantity?: FormControl<number>;
}

@Component({
  selector: 'app-create-shopping-list-item',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './create-shopping-list-item.component.html',
  styleUrl: './create-shopping-list-item.component.css',
})
export class CreateShoppingListItemComponent {
  private readonly shoppingListService = inject(ShoppingListService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);
  private priceValueChanges?: Subscription;

  public isSubmitting = false;
  public readonly priceRegEx = new RegExp(/^[0-9]{1,10}(\.[0-9]{0,2})?$/);

  @Output() canceled = new EventEmitter();
  @Output() created = new EventEmitter();

  createShoppingItemForm: FormGroup<CreateShoppingItemForm> =
    this.formBuilder.nonNullable.group({
      name: this.formBuilder.nonNullable.control(''),
    });

  get name() {
    return this.createShoppingItemForm.controls.name;
  }

  get measurement() {
    return this.createShoppingItemForm.controls.measurement;
  }

  get price() {
    return this.createShoppingItemForm.controls.price;
  }

  get quantity() {
    return this.createShoppingItemForm.controls.quantity;
  }

  addMeasurement() {
    this.createShoppingItemForm.addControl(
      'measurement',
      this.formBuilder.nonNullable.control(''),
    );
  }

  addPrice() {
    this.createShoppingItemForm.addControl(
      'price',
      this.formBuilder.nonNullable.control(0),
    );

    const priceControl = this.price!;

    this.priceValueChanges = priceControl.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        if (priceControl.invalid) {
          priceControl.setValue(0, { emitEvent: false });
        }
      });
  }

  addQuantity() {
    this.createShoppingItemForm.addControl(
      'quantity',
      this.formBuilder.nonNullable.control(1),
    );
  }

  removeMeasurement() {
    this.createShoppingItemForm.removeControl('measurement');
  }

  removePrice() {
    this.priceValueChanges?.unsubscribe();
    this.createShoppingItemForm.removeControl('price');
  }

  removeQuantity() {
    this.createShoppingItemForm.removeControl('quantity');
  }

  cancel() {
    this.canceled.emit();
  }

  submit() {
    this.isSubmitting = true;

    if (this.createShoppingItemForm.invalid) {
      return;
    }

    const request = this.fromForm();
    this.shoppingListService
      .createShoppingItem(request)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((success) => {
        if (success) {
          this.created.emit();
        }
        this.isSubmitting = false;
      });
  }

  private fromForm(): CreateShoppingItemRequest {
    return this.createShoppingItemForm.getRawValue();
  }
}
