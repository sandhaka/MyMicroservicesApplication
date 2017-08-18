import {Component, OnInit} from '@angular/core';
import {BasketService} from "./basket.service";
import {BasketData} from "./basket-data";

@Component({
  selector: 'basket',
  templateUrl: 'basket.component.html'
})

export class BasketComponent implements OnInit {

  basket: BasketData;

  private readonly basketService: BasketService;

  constructor(basketService: BasketService) {
    this.basketService = basketService;
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
}
