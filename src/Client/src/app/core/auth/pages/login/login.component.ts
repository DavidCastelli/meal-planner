import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import {
  ReactiveFormsModule,
  FormGroup,
  FormControl,
  Validators,
  FormBuilder,
} from '@angular/forms';
import { AuthService } from '../../auth.service';
import { catchError, EMPTY } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly router = inject(Router);
  private readonly formBuilder = inject(FormBuilder);

  loginForm!: FormGroup<{
    email: FormControl<string>;
    password: FormControl<string>;
  }>;

  errorMessage: string | null = null;
  isSubmitting = false;

  ngOnInit() {
    this.loginForm = this.formBuilder.nonNullable.group({
      email: ['', { validators: [Validators.required] }],
      password: ['', { validators: [Validators.required] }],
    });
  }

  get email() {
    return this.loginForm.controls.email;
  }

  get password() {
    return this.loginForm.controls.password;
  }

  onSubmit() {
    this.isSubmitting = true;
    this.errorMessage = null;

    if (this.loginForm.invalid) {
      // Mark all as touched to handle the case where the user submits without interacting with the inputs.
      this.loginForm.markAllAsTouched();
      this.isSubmitting = false;
      return;
    }

    const credentials = this.loginForm.getRawValue(); // Assumes controls are never disabled.
    this.authService
      .login(credentials)
      .pipe(
        catchError((err) => {
          this.errorMessage = err;
          this.isSubmitting = false;
          return EMPTY;
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({ next: () => void this.router.navigate(['/home']) });
  }
}
