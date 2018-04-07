import {Injectable} from '@angular/core';
import {Http, Headers, RequestOptions, Response} from "@angular/http";
import {AuthenticationService} from "../core/security/authentication.service";
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map'
import {BasketData} from "./basket-data";
import {environment} from "../../environments/environment";

@Injectable()
export class BasketService {

  private http: Http;
  private authService: AuthenticationService;

  constructor(http: Http, authService: AuthenticationService) {
    this.http = http;
    this.authService = authService;
  }

  getBasket() : Observable<any> {
    return this.http.get(
      environment.settings.basket_gateway + `/api/basket`,
      this.getOptions())
      .map((response: Response) => response.json());
  }

  updateBasket(basket: BasketData) : Observable<any> {
    return this.http.post(
      environment.settings.basket_gateway + '/api/basket',
      basket,
      this.getOptions())
      .map((response: Response) => response.json());
  }

  deleteBasket() : Observable<any> {
    return this.http.delete(
      environment.settings.basket_gateway + `/api/basket`,
      this.getOptions())
      .map((response: Response) => response.json());
  }

  private getOptions() : RequestOptions {
    // add authorization header with jwt token
    let headers = new Headers({ 'Authorization': 'Bearer ' + this.authService.token });
    return new RequestOptions({ headers: headers });
  }
}
