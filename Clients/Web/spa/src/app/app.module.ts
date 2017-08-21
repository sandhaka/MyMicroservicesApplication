import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AppComponent } from './app.component';
import {RouterModule} from '@angular/router';
import {appRoutes} from "./app.routes";
import {LoginModule} from "./login/login.module";
import {ShopModule} from "./shop/shop.module";
import {CoreModule} from "./core/core.module";
import {OrdersModule} from "./orders/orders.module";

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    NgbModule.forRoot(),
    RouterModule.forRoot(
      appRoutes
    ),
    CoreModule,
    LoginModule,
    ShopModule,
    OrdersModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
