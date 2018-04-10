import {RouterModule, Routes} from "@angular/router";
import {LoginComponent} from "./login/login.component";
import {ShopComponent} from "./shop/shop.component";
import {AuthGuardService} from "./core/security/auth.guard.service";
import {NgModule} from "@angular/core";
import {OrdersComponent} from "./orders/orders.component";
import {BasketComponent} from "./basket/basket.component";

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
    path: 'checkout',
    component: OrdersComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: 'basket',
    component: BasketComponent,
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

@NgModule({
  imports: [ RouterModule.forRoot(appRoutes) ],
  exports: [ RouterModule ]
})
export class AppRoutingModule {}
