import { Component, DestroyRef, inject, Input, OnInit } from '@angular/core';
import { FormGroupDirective, ValidationErrors } from '@angular/forms';
import {
  BehaviorSubject,
  distinctUntilChanged,
  merge,
  Subscription,
} from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { FORM_ERRORS } from '../../configs/error.config';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-control-error',
  standalone: true,
  imports: [AsyncPipe],
  templateUrl: './control-error.component.html',
  styleUrl: './control-error.component.css',
})
export class ControlErrorComponent implements OnInit {
  private subscription = new Subscription();
  private readonly formGroupDirective = inject(FormGroupDirective);
  private readonly errors = inject(FORM_ERRORS);
  private readonly destroyRef = inject(DestroyRef);

  readonly message$ = new BehaviorSubject<string>('');

  @Input({ required: true }) controlPath!: string;
  @Input() customErrors?: ValidationErrors;

  ngOnInit() {
    const control = this.formGroupDirective.control.get(this.controlPath);

    if (control) {
      this.subscription = merge(
        control.valueChanges,
        this.formGroupDirective.ngSubmit,
      )
        .pipe(distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
        .subscribe(() => {
          const controlErrors = control.errors;

          if (controlErrors) {
            const firstKey = Object.keys(controlErrors)[0];
            const getError = this.errors[firstKey];
            const text =
              this.customErrors?.[firstKey] ||
              getError(controlErrors[firstKey]);

            this.setError(text);
          } else {
            this.setError('');
          }
        });
    } else {
      console.error(
        `Control "${this.controlPath}" not found in the form group.`,
      );
    }
  }

  private setError(text: string) {
    this.message$.next(text);
  }
}
