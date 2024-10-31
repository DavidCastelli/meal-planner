import { Component, inject } from '@angular/core';
import { ErrorService } from '../../../core/errors/error.service';

@Component({
  selector: 'app-form-errors',
  standalone: true,
  imports: [],
  templateUrl: './form-errors.component.html',
  styleUrl: './form-errors.component.css',
})
export class FormErrorsComponent {
  public readonly errorService = inject(ErrorService);
}
