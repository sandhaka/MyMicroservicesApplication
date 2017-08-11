import {NgModule} from '@angular/core';
import {ShopComponent} from './shop.component';
import {CoreModule} from "../core/core.module";

@NgModule({
  imports: [
    CoreModule
  ],
  exports: [],
  declarations: [ShopComponent],
  providers: [],
})
export class ShopModule {
}
