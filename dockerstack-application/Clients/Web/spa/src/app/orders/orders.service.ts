import {Injectable} from '@angular/core';
import {Http, Response} from "@angular/http";
import {AuthenticationService} from "../core/security/authentication.service";
import {UtilityService} from "../core/utils.service";
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map'
import {CreateOrderCommand} from "./order-command";
import {environment} from "../../environments/environment";

@Injectable()
export class OrdersService {

  private http: Http;
  private authService: AuthenticationService;

  constructor(
    http: Http,
    authService: AuthenticationService) {
    this.http = http;
    this.authService = authService;
  }

  placeOrder(createOrderCommand: CreateOrderCommand) : Observable<Response> {
    return this.http.post(
      environment.settings.orders_gateway + '/api/orders/new',
      createOrderCommand,
      UtilityService.getRequestOptions(this.authService.token)
    )
      .map((response: Response) => response);
  }
}
