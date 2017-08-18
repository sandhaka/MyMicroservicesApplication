import {Component, OnInit} from '@angular/core';
import {ShopService} from "./shop.service";
import {AuthenticationService} from "../core/security/authentication.service";
import {Router} from "@angular/router";
import {BasketService} from "../basket/basket.service";
import {BasketData} from "../basket/basket-data";
import {ProductData} from "./product-data";
import {forEach} from "@angular/router/src/utils/collection";

@Component({
  selector: 'shop-component',
  templateUrl: 'shop.component.html',
  styleUrls: ['shop.component.css']
})

export class ShopComponent implements OnInit {

  products: ProductData[];
  basket: BasketData;

  private shopService: ShopService;
  private viewType: string = "grid";
  private authService: AuthenticationService;
  private router: Router;
  private basketService: BasketService;

  constructor(
    shopService: ShopService,
    authService: AuthenticationService,
    router: Router,
    basketService: BasketService) {
    this.shopService = shopService;
    this.authService = authService;
    this.router = router;
    this.basketService = basketService;

    this.basket = new BasketData(this.authService.getCurrentUserId());
    this.products = [];
  }

  ngOnInit() {
    this.shopService.getProducts().subscribe((data) => {
        data.forEach(item => {
          this.products.push(new ProductData(
            item.id,
            item.productName,
            item.unitPrice,
            item.package,
            item.assets));
        });
      },
      (error) => {
        console.error(error);
      });

    this.basketService.getBasket().subscribe((data) => {
        if(data.result !== null) {
          this.basket.copyFrom(data.result);
        }
      },
      (error) => {
        console.error(error);
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

  addToBasket(product: ProductData) {

    this.basket.addItem(product.Id,product.ProductName,product.UnitPrice,1);

    this.basketService.updateBasket(this.basket).subscribe((data) => {
      if(data.result !== null) {
        this.basket.copyFrom(data.result);
      }
    },(error) => {
      console.error(error);
    });
  }

  goToBasket() {
    this.router.navigate(['/basket']);
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
