import {NgModule} from "@angular/core";
import {CoreModule} from "../core/core.module";
import {OrdersComponent} from "./orders.component";
import {OrdersService} from "./orders.service";
import {RouterModule} from "@angular/router";
import {ordersRoutes} from "./orders.routes";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {OrderConfirmationComponent} from "./order-confirmation.component";

@NgModule({
  imports: [
    CoreModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot(
      ordersRoutes
    )
  ],
  exports: [],
  declarations: [
    OrdersComponent,
    OrderConfirmationComponent
  ],
  providers: [OrdersService]
})
export class OrdersModule {}
