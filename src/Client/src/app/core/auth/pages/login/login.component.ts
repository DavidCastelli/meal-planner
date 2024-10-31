import { Component, DestroyRef, inject } from '@angular/core';
import { ReactiveFormsModule, Validators, FormBuilder } from '@angular/forms';
import { AuthService } from '../../auth.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { ControlErrorComponent } from '../../../../shared/components/control-error/control-error.component';
import { ErrorService } from '../../../errors/error.service';
import { FormErrorsComponent } from '../../../../shared/components/form-errors/form-errors.component';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, ControlErrorComponent, FormErrorsComponent],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  private readonly authService = inject(AuthService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly router = inject(Router);
  private readonly formBuilder = inject(FormBuilder);

  public readonly errorService = inject(ErrorService);

  loginForm = this.formBuilder.nonNullable.group({
    email: ['', { validators: [Validators.required] }],
    password: ['', { validators: [Validators.required] }],
  });

  isSubmitting = false;

  onSubmit() {
    this.isSubmitting = true;
    this.errorService.clear();

    if (this.loginForm.invalid) {
      this.isSubmitting = false;
      return;
    }

    const credentials = this.loginForm.getRawValue(); // Assumes controls are never disabled.
    this.authService
      .login(credentials)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((success) => {
        if (success) {
          void this.router.navigate(['/home']);
        } else {
          this.isSubmitting = false;
        }
      });
  }
}
