import {Injectable} from '@angular/core';
import {Http, Headers, RequestOptions, Response} from "@angular/http";
import {ServerConfigurationService} from "../core/server-configuration.service";
import {AuthenticationService} from "../core/security/authentication.service";
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map'
import {BasketData} from "./basket-data";

@Injectable()
export class BasketService {

  private http: Http;
  private authService: AuthenticationService;
  private serverConfig: ServerConfigurationService;

  constructor(http: Http, serverConfigurationService: ServerConfigurationService, authService: AuthenticationService) {
    this.http = http;
    this.serverConfig = serverConfigurationService;
    this.authService = authService;
  }

  getBasket() : Observable<any> {
    return this.http.get(
      this.serverConfig.basketServer + `/api/basket/${this.authService.getCurrentUserId()}`,
      this.getOptions())
      .map((response: Response) => response.json());
  }

  updateBasket(basket: BasketData) : Observable<any> {
    return this.http.post(
      this.serverConfig.basketServer + '/api/basket',
      basket,
      this.getOptions())
      .map((response: Response) => response.json());
  }

  deleteBasket() : Observable<any> {
    return this.http.delete(
      this.serverConfig.basketServer + `/api/basket/${this.authService.getCurrentUserId()}`,
      this.getOptions())
      .map((response: Response) => response.json());
  }

  private getOptions() : RequestOptions {
    // add authorization header with jwt token
    let headers = new Headers({ 'Authorization': 'Bearer ' + this.authService.token });
    return new RequestOptions({ headers: headers });
  }
}
