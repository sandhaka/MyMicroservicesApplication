import {AuthGuardService} from "../core/security/auth.guard.service";
import {Routes} from "@angular/router";
import {OrdersComponent} from "./orders.component";

export const ordersRoutes: Routes = [
  {
    path: 'checkout',
    component: OrdersComponent,
    canActivate: [AuthGuardService]
  }
];
