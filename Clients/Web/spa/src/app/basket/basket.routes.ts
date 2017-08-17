import {Routes} from "@angular/router";
import {BasketComponent} from "./basket.component";
import {AuthGuardService} from "../core/security/auth.guard.service";

export const basketRoutes: Routes = [
  {
    path: 'basket',
    component: BasketComponent,
    canActivate: [AuthGuardService]
  }
];
