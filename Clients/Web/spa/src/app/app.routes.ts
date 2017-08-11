import {Routes} from "@angular/router";
import {LoginComponent} from "./login/login.component";
import {ShopComponent} from "./shop/shop.component";
import {AuthGuardService} from "./core/security/auth.guard.service";

export const appRoutes: Routes = [
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'shop',
    component: ShopComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: '',
    component: LoginComponent
  },
  {
    path: '**',
    redirectTo: ''
  }
];
