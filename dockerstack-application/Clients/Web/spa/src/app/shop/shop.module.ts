import {NgModule} from '@angular/core';
import {ShopComponent} from './shop.component';
import {CoreModule} from "../core/core.module";
import {ShopService} from "./shop.service";
import {CommonModule} from "@angular/common";
import {BasketModule} from "../basket/basket.module";

@NgModule({
  imports: [
    CoreModule,
    BasketModule,
    CommonModule
  ],
  exports: [],
  declarations: [ShopComponent],
  providers: [ShopService],
})
export class ShopModule {
}
