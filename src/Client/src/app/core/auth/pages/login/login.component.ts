import { Component, DestroyRef, inject } from '@angular/core';
import {
  ReactiveFormsModule,
  FormGroup,
  FormControl,
  Validators,
} from '@angular/forms';
import { AuthService } from '../../services/auth.service';
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
export class LoginComponent {
  private readonly authService = inject(AuthService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly router = inject(Router);

  errorMessage: string | null = null;
  isSubmitting = false;

  loginForm = new FormGroup({
    email: new FormControl('', {
      validators: [Validators.required],
      nonNullable: true,
    }),
    password: new FormControl('', {
      validators: [Validators.required],
      nonNullable: true,
    }),
  });

  onSubmit() {
    this.isSubmitting = true;

    if (this.loginForm.invalid) {
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
