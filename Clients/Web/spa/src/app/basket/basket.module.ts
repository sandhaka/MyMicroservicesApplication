import {NgModule} from '@angular/core';

import {BasketComponent} from './basket.component';
import {BasketService} from "./basket.service";
import {CoreModule} from "../core/core.module";
import {RouterModule} from "@angular/router";
import {basketRoutes} from "./basket.routes";
import {CommonModule} from "@angular/common";

@NgModule({
  imports: [
    CoreModule,
    CommonModule,
    RouterModule.forRoot(
      basketRoutes
    )
  ],
  exports: [],
  declarations: [BasketComponent],
  providers: [BasketService],
})
export class BasketModule { }
