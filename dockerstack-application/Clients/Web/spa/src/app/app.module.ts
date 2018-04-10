import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AppComponent } from './app.component';
import {AppRoutingModule} from "./app.routes";
import {LoginModule} from "./login/login.module";
import {ShopModule} from "./shop/shop.module";
import {CoreModule} from "./core/core.module";
import {OrdersModule} from "./orders/orders.module";
import {BasketModule} from "./basket/basket.module";
import {HTTP_INTERCEPTORS, HttpClientModule} from "@angular/common/http";
import {JwtIntegrationHttpInterceptor} from "./core/security/jwt-integration-http.interceptor";

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    HttpClientModule,
    BrowserModule,
    NgbModule.forRoot(),
    AppRoutingModule,
    CoreModule,
    LoginModule,
    ShopModule,
    OrdersModule,
    BasketModule
  ],
  providers: [
    {
      provide:HTTP_INTERCEPTORS,
      useClass: JwtIntegrationHttpInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
