import {Injectable} from '@angular/core';
import {Http, Response} from "@angular/http";
import {AuthenticationService} from "../core/security/authentication.service";
import {ServerConfigurationService} from "../core/server-configuration.service";
import {UtilityService} from "../core/utils.service";
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map'
import {CreateOrderCommand} from "./order-command";

@Injectable()
export class OrdersService {

  private http: Http;
  private authService: AuthenticationService;
  private serverConfig: ServerConfigurationService;

  constructor(
    http: Http,
    serverConfigurationService: ServerConfigurationService,
    authService: AuthenticationService) {
    this.http = http;
    this.serverConfig = serverConfigurationService;
    this.authService = authService;
  }

  placeOrder(createOrderCommand: CreateOrderCommand) : Observable<Response> {
    return this.http.post(
      this.serverConfig.ordersServer + '/api/orders/new',
      createOrderCommand,
      UtilityService.getRequestOptions(this.authService.token)
    )
      .map((response: Response) => response);
  }
}
