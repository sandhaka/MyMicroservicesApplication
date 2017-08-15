import {Component, OnInit} from '@angular/core';
import {ShopService} from "./shop.service";
import {AuthenticationService} from "../core/security/authentication.service";
import {Router} from "@angular/router";

@Component({
  selector: 'shop-component',
  templateUrl: 'shop.component.html',
  styleUrls: ['shop.component.css']
})

export class ShopComponent implements OnInit {

  products: any;

  private shopService: ShopService;
  private viewType: string = "grid";
  private authService: AuthenticationService;
  private router: Router;

  constructor(shopService: ShopService, authService: AuthenticationService, router: Router) {
    this.shopService = shopService;
    this.authService = authService;
    this.router = router;
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

  goToBasket() {
    // TODO
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
