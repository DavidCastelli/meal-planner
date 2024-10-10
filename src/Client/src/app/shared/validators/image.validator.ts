import { AbstractControl, ValidationErrors } from '@angular/forms';

export class ImageValidator {
  public static empty(control: AbstractControl): ValidationErrors | null {
    if (control.value === null) {
      return null;
    }

    const image = control.value as File;

    return image.size > 0 ? null : { empty: true };
  }

  public static max(control: AbstractControl): ValidationErrors | null {
    if (control.value === null) {
      return null;
    }

    const image = control.value as File;

    return image.size <= 5242880
      ? null
      : { size: { max: 5242880, actual: image.size } };
  }

  public static extensions(control: AbstractControl): ValidationErrors | null {
    if (control.value === null) {
      return null;
    }

    const image = control.value as File;
    const permittedExtensions = ['image/jpeg', 'image/jpg'];

    const isPermittedExtension = permittedExtensions.includes(image.type);

    return isPermittedExtension
      ? null
      : { permitted: ['image/jpeg', 'image/jpg'], actual: image.type };
  }
}
