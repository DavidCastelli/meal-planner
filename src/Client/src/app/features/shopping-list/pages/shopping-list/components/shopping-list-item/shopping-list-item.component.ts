import {
  Component,
  DestroyRef,
  EventEmitter,
  HostListener,
  inject,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import { GetShoppingItemsDto } from '../../../../models/get-shopping-items-dto.model';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
} from '@angular/forms';
import { UpdateShoppingItemRequest } from '../../../../models/update-shopping-item-request.model';
import { debounceTime, filter, Subscription, switchMap, tap } from 'rxjs';
import { ShoppingListService } from '../../../../shopping-list.service';
import { ShoppingListItemService } from '../../../../shopping-list-item.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

interface UpdateShoppingItemForm {
  name: FormControl<string>;
  measurement?: FormControl<string>;
  price?: FormControl<number>;
  quantity?: FormControl<number>;
  checked: FormControl<boolean>;
  locked: FormControl<boolean>;
}

@Component({
  selector: 'app-shopping-list-item',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './shopping-list-item.component.html',
  styleUrl: './shopping-list-item.component.css',
})
export class ShoppingListItemComponent implements OnInit {
  private readonly shoppingListService = inject(ShoppingListService);
  private readonly shoppingListItemService = inject(ShoppingListItemService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);
  private priceValueChanges?: Subscription;

  public editMode = false;
  public isSaving = false;
  public saveMessage = '';
  public readonly priceRegEx = new RegExp(/^[0-9]{1,10}(\.[0-9]{0,2})?$/);

  @Input({ required: true }) shoppingItem!: GetShoppingItemsDto;

  @Output() update: EventEmitter<UpdateShoppingItemRequest> =
    new EventEmitter();
  @Output() deleted: EventEmitter<number> = new EventEmitter();

  @HostListener('click', ['$event'])
  selectShoppingItem(event: Event) {
    if (
      event.target instanceof HTMLButtonElement ||
      event.target instanceof HTMLInputElement) {
      return;
    }

    this.shoppingListItemService.setSelected(this.shoppingItem.id);
  }

  shoppingItemForm: FormGroup<UpdateShoppingItemForm> =
    this.formBuilder.nonNullable.group({
      name: this.formBuilder.nonNullable.control(''),
      checked: this.formBuilder.nonNullable.control(false),
      locked: this.formBuilder.nonNullable.control(false),
    });

  ngOnInit() {
    if (this.shoppingItem.measurement !== undefined) {
      this.addMeasurement();
    }

    if (this.shoppingItem.price !== undefined) {
      this.addPrice();
    }

    if (this.shoppingItem.quantity !== undefined) {
      this.addQuantity();
    }

    this.shoppingItemForm.patchValue({
      name: this.shoppingItem.name,
      measurement: this.shoppingItem.measurement,
      price: this.shoppingItem.price,
      quantity: this.shoppingItem.quantity,
      checked: this.shoppingItem.isChecked,
      locked: this.shoppingItem.isLocked,
    });

    this.disable();
    this.autoSave();
    this.selected();
  }

  get name() {
    return this.shoppingItemForm.controls.name;
  }

  get measurement() {
    return this.shoppingItemForm.controls.measurement;
  }

  get price() {
    return this.shoppingItemForm.controls.price;
  }

  get quantity() {
    return this.shoppingItemForm.controls.quantity;
  }

  get checked() {
    return this.shoppingItemForm.controls.checked;
  }

  get locked() {
    return this.shoppingItemForm.controls.locked;
  }

  addMeasurement() {
    this.shoppingItemForm.addControl(
      'measurement',
      this.formBuilder.nonNullable.control(''),
    );
  }

  addPrice() {
    this.shoppingItemForm.addControl(
      'price',
      this.formBuilder.nonNullable.control(0),
    );

    const priceControl = this.price!;

    if (!this.priceValueChanges) {
      this.priceValueChanges = priceControl.valueChanges
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe(() => {
          if (priceControl.invalid) {
            priceControl.setValue(0, { emitEvent: false });
          }
        });
    }
  }

  addQuantity() {
    this.shoppingItemForm.addControl(
      'quantity',
      this.formBuilder.nonNullable.control(1),
    );
  }

  removeMeasurement() {
    this.shoppingItemForm.removeControl('measurement');
  }

  removePrice() {
    this.priceValueChanges?.unsubscribe();
    this.shoppingItemForm.removeControl('price');
  }

  removeQuantity() {
    this.shoppingItemForm.removeControl('quantity');
  }

  delete() {
    this.deleted.emit(this.shoppingItem.id);
  }

  private selected() {
    this.shoppingListItemService.selected$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((id) => {
        if (id === this.shoppingItem.id) {
          this.editMode = !this.editMode;
          if (this.editMode) {
            const priceControl = this.price;

            if (priceControl) {
              this.priceValueChanges = priceControl.valueChanges
                .pipe(takeUntilDestroyed(this.destroyRef))
                .subscribe(() => {
                  if (priceControl.invalid) {
                    priceControl.setValue(0, { emitEvent: false });
                  }
                });
            }

            this.editMode = true;
            this.enable();
          } else {
            this.priceValueChanges?.unsubscribe();
            this.editMode = false;
            this.disable();
          }
        } else {
          this.priceValueChanges?.unsubscribe();
          this.editMode = false;
          this.disable();
        }
      });
  }

  private autoSave() {
    this.shoppingItemForm.valueChanges
      .pipe(
        debounceTime(2000),
        filter(() => this.shoppingItemForm.valid),
        switchMap(() => {
          this.isSaving = true;
          this.saveMessage = 'Saving...';

          const request = this.fromForm();

          return this.shoppingListService.updateShoppingItem(
            request.id,
            request,
          );
        }),
        tap((success) => {
          if (success) {
            const request = this.fromForm();
            this.update.emit(request);
          }
        }),
        debounceTime(1000),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((success) => {
        if (success) {
          this.isSaving = false;
          this.saveMessage = 'Saved';
          setTimeout(() => {
            this.saveMessage = '';
          }, 1500);
        }
      });
  }

  private fromForm(): UpdateShoppingItemRequest {
    const values = this.shoppingItemForm.getRawValue();
    return {
      id: this.shoppingItem.id,
      name: values.name,
      measurement: values.measurement,
      price: values.price,
      quantity: values.quantity,
      isChecked: values.checked,
      isLocked: values.locked,
    };
  }

  private enable() {
    this.name.enable({ emitEvent: false });
    this.measurement?.enable({ emitEvent: false });
    this.price?.enable({ emitEvent: false });
    this.quantity?.enable({ emitEvent: false });
  }

  private disable() {
    this.name.disable({ emitEvent: false });
    this.measurement?.disable({ emitEvent: false });
    this.price?.disable({ emitEvent: false });
    this.quantity?.disable({ emitEvent: false });
  }
}
