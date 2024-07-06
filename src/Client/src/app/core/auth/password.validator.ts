import { AbstractControl, ValidationErrors } from '@angular/forms';

export class PasswordValidator {
  /**
   * Validator that requires the control's value consist of at least 1 uppercase letter, one lowercase letter, one numeric digit, and one nonalphanumeric character.
   * The validator is intended to match the password requirements configured for ASP.NET Core Identity /register endpoint.
   * Is considered valid when the control value is the empty string to stay consistent with other validators.
   * The required validator should be used in the case an empty value should not be considered valid.
   *
   * @param control - The control to be validated.
   * @returns An error map with the strong property if the validation check fails, otherwise null.
   */
  public static strong(control: AbstractControl): ValidationErrors | null {
    if (control.value === '') return null;

    const hasUpper = /[A-Z]/.test(control.value);
    const hasLower = /[a-z]/.test(control.value);
    const hasNumber = /[0-9]/.test(control.value);
    const hasNonAlphaNumeric = /[^a-zA-Z0-9]/.test(control.value);

    const isValid = hasUpper && hasLower && hasNumber && hasNonAlphaNumeric;

    return isValid ? null : { strong: true };
  }
}
