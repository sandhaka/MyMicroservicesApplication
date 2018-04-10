import {NgModule} from '@angular/core';
import {BasketComponent} from './basket.component';
import {BasketService} from "./basket.service";
import {CoreModule} from "../core/core.module";
import {CommonModule} from "@angular/common";

@NgModule({
  imports: [
    CoreModule,
    CommonModule
  ],
  exports: [],
  declarations: [BasketComponent],
  providers: [BasketService],
})
export class BasketModule { }
