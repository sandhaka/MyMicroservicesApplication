import {NgModule} from '@angular/core';
import {ShopComponent} from './shop.component';
import {CoreModule} from "../core/core.module";
import {ShopService} from "./shop.service";
import {CommonModule} from "@angular/common";

@NgModule({
  imports: [
    CoreModule,
    CommonModule
  ],
  exports: [],
  declarations: [ShopComponent],
  providers: [ShopService],
})
export class ShopModule {
}
