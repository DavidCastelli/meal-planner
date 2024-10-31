import { Component, DestroyRef, inject } from '@angular/core';
import { ReactiveFormsModule, Validators, FormBuilder } from '@angular/forms';
import { AuthService } from '../../auth.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { PasswordValidator } from '../../password.validator';
import { Router } from '@angular/router';
import { ControlErrorComponent } from '../../../../shared/components/control-error/control-error.component';
import { ErrorService } from '../../../errors/error.service';
import { FormErrorsComponent } from '../../../../shared/components/form-errors/form-errors.component';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, ControlErrorComponent, FormErrorsComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  private readonly authService = inject(AuthService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly router: Router = inject(Router);
  private readonly formBuilder = inject(FormBuilder);

  public readonly errorService = inject(ErrorService);

  registrationForm = this.formBuilder.nonNullable.group({
    email: ['', { validators: [Validators.required, Validators.email] }],
    password: [
      '',
      {
        validators: [
          Validators.required,
          Validators.minLength(6),
          PasswordValidator.strong,
        ],
      },
    ],
  });

  isSubmitting = false;

  onSubmit() {
    this.isSubmitting = true;
    this.errorService.clear();

    if (this.registrationForm.invalid) {
      this.isSubmitting = false;
      return;
    }

    const credentials = this.registrationForm.getRawValue(); // Assumes controls are never disabled.
    this.authService
      .register(credentials)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((success) => {
        if (success) {
          void this.router.navigate(['/login']);
        } else {
          this.registrationForm.reset();
          this.isSubmitting = false;
        }
      });
  }
}
