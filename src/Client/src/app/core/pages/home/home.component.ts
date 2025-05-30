import {Component, inject} from '@angular/core';
import {slideAnimation} from "../../../shared/animations/slide.animation";
import {SidebarService} from "../../layout/sidebar.service";
import {AsyncPipe} from "@angular/common";
import {Router} from "@angular/router";

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    AsyncPipe,
  ],
  animations: [slideAnimation],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent {
  private readonly sidebarService = inject(SidebarService);
  private readonly router = inject(Router);

  public readonly isSidebarOpen$ = this.sidebarService.openClose$;

  navigateRecipes() {
    void this.router.navigate(['/manage/recipes']);
  }

  navigateMeals() {
    void this.router.navigate(['/manage/meals']);
  }

  navigateSchedule() {
    void this.router.navigate(['/schedule']);
  }

  navigateShoppingList() {
    void this.router.navigate(['/shopping-list']);
  }
}
