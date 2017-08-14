import {Component, OnInit} from '@angular/core';
import {ShopService} from "./shop.service";

@Component({
  selector: 'shop-component',
  templateUrl: 'shop.component.html',
  styleUrls: ['shop.component.css']
})

export class ShopComponent implements OnInit {

  products: any;

  private shopService: ShopService;
  private viewType: string = "grid";

  constructor(shopService: ShopService) {
    this.shopService = shopService;
  }

  ngOnInit() {
    this.shopService.getProducts().subscribe((data) => {
      this.products = data;
    });
  }

  viewByList(event) {
    event.preventDefault();
    this.viewType = 'list';
  }

  viewByGrid(event) {
    event.preventDefault();
    this.viewType = 'grid';
  }

  AddToBasket() {
    // TODO
  }
}
