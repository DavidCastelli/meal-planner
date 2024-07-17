import { Component, DestroyRef, inject } from '@angular/core';
import {
  ReactiveFormsModule,
  FormGroup,
  FormControl,
  Validators,
} from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { PasswordValidator } from '../../password.validator';
import { catchError, EMPTY } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  private readonly authService = inject(AuthService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly router: Router = inject(Router);

  errorMessage: string | null = null;
  isSubmitting = false;

  registrationForm = new FormGroup({
    email: new FormControl('', {
      validators: [Validators.required, Validators.email],
      nonNullable: true,
    }),
    password: new FormControl('', {
      validators: [
        Validators.required,
        Validators.minLength(6),
        PasswordValidator.strong,
      ],
      nonNullable: true,
    }),
  });

  get email() {
    return this.registrationForm.controls.email;
  }

  get password() {
    return this.registrationForm.controls.password;
  }

  onSubmit() {
    this.isSubmitting = true;

    if (this.registrationForm.invalid) {
      // Mark all as touched to handle the case where the user submits without interacting with the inputs.
      this.registrationForm.markAllAsTouched();
      this.isSubmitting = false;
      return;
    }

    const credentials = this.registrationForm.getRawValue(); // Assumes controls are never disabled.
    this.authService
      .register(credentials)
      .pipe(
        catchError((err) => {
          this.errorMessage = err;
          this.registrationForm.reset();
          this.isSubmitting = false;
          return EMPTY;
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({ next: () => void this.router.navigate(['/login']) });
  }
}
