import { Component, inject } from '@angular/core';
import { slideAnimation } from '../../../shared/animations/slide.animation';
import { SidebarService } from '../../layout/sidebar.service';
import { AsyncPipe } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [AsyncPipe, RouterLink],
  animations: [slideAnimation],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent {
  private readonly sidebarService = inject(SidebarService);

  public readonly isSidebarOpen$ = this.sidebarService.openClose$;
}
