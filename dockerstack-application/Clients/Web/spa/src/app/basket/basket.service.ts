import {Injectable} from '@angular/core';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map'
import {BasketData} from "./basket-data";
import {environment} from "../../environments/environment";
import {HttpClient} from "@angular/common/http";

@Injectable()
export class BasketService {

  constructor(private http: HttpClient) {  }

  getBasket() : Observable<any> {
    return this.http.get(environment.settings.basket_gateway + `/api/basket`);
  }

  updateBasket(basket: BasketData) : Observable<any> {
    return this.http.post(environment.settings.basket_gateway + '/api/basket', basket);
  }

  deleteBasket() : Observable<any> {
    return this.http.delete(environment.settings.basket_gateway + `/api/basket`);
  }
}
