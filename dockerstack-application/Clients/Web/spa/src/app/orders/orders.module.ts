import {NgModule} from "@angular/core";
import {CoreModule} from "../core/core.module";
import {OrdersComponent} from "./orders.component";
import {OrdersService} from "./orders.service";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {OrderConfirmationComponent} from "./order-confirmation.component";
import {RouterModule} from "@angular/router";

@NgModule({
  imports: [
    CoreModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule
  ],
  exports: [],
  declarations: [
    OrdersComponent,
    OrderConfirmationComponent
  ],
  providers: [OrdersService]
})
export class OrdersModule {}
