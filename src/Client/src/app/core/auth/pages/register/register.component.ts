import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import {
  ReactiveFormsModule,
  FormGroup,
  FormControl,
  Validators,
  FormBuilder,
} from '@angular/forms';
import { AuthService } from '../../auth.service';
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
export class RegisterComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly router: Router = inject(Router);
  private readonly formBuilder = inject(FormBuilder);

  registrationForm!: FormGroup<{
    email: FormControl<string>;
    password: FormControl<string>;
  }>;

  errorMessage: string | null = null;
  isSubmitting = false;

  ngOnInit() {
    this.registrationForm = this.formBuilder.nonNullable.group({
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
  }

  get email() {
    return this.registrationForm.controls.email;
  }

  get password() {
    return this.registrationForm.controls.password;
  }

  onSubmit() {
    this.isSubmitting = true;
    this.errorMessage = null;

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
