import {Component, OnInit} from '@angular/core';
import {BasketService} from "./basket.service";
import {BasketData} from "./basket-data";
import {AuthenticationService} from "../core/security/authentication.service";
import {Router} from "@angular/router";

@Component({
  selector: 'basket',
  templateUrl: 'basket.component.html'
})

export class BasketComponent implements OnInit {

  basket: BasketData;

  private readonly basketService: BasketService;
  private authService: AuthenticationService;
  private router: Router;

  constructor(
    basketService: BasketService,
    authService: AuthenticationService,
    router: Router) {
    this.basketService = basketService;
    this.authService = authService;
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
}
