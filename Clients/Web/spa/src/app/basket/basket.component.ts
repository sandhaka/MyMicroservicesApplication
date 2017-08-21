import {Component, OnInit} from '@angular/core';
import {BasketService} from "./basket.service";
import {BasketData} from "./basket-data";
import {AuthenticationService} from "../core/security/authentication.service";
import {Router} from "@angular/router";
import {OrdersService} from "../orders/orders.service";
import {ProductData} from "../core/shared/product-data";

@Component({
  selector: 'basket',
  templateUrl: 'basket.component.html'
})

export class BasketComponent implements OnInit {

  basket: BasketData;

  private readonly basketService: BasketService;
  private readonly ordersService: OrdersService;
  private authService: AuthenticationService;
  private router: Router;

  constructor(
    basketService: BasketService,
    authService: AuthenticationService,
    ordersService: OrdersService,
    router: Router) {
    this.basketService = basketService;
    this.authService = authService;
    this.ordersService = ordersService;
    this.router = router;
    this.basket = new BasketData("");

  }

  ngOnInit() {
    this.basketService.getBasket().subscribe((data) => {
        if(data.result !== null) {
          this.basket.copyFrom(data.result);
        }
      },
      (error) => {
        console.error(error);
      });
  }

  deleteCurrentBasket() {
    this.basketService.deleteBasket().subscribe((data) => {
        if(data) {
          if(data.result) {
            this.basket.basketItems = [];
          }
        }
      },
      (error) => {
        console.error(error);
      });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  checkoutOrder() {
    let orderItems = [];
    this.basket.basketItems.forEach(item => {
      orderItems.push({
        productId: item.productId,
        productName: item.productName,
        unitPrice: item.unitPrice,
        units: item.units});
    });
    this.router.navigate(['/checkout']);
  }
}
