import {Injectable} from '@angular/core';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map'
import {CreateOrderCommand} from "./order-command";
import {environment} from "../../environments/environment";
import {HttpClient} from "@angular/common/http";

@Injectable()
export class OrdersService {

  constructor(private http: HttpClient) {  }

  placeOrder(createOrderCommand: CreateOrderCommand) : Observable<Response> {
    return this.http.post(
      environment.settings.orders_gateway + '/api/orders/new',
      createOrderCommand
    )
      .map((response: Response) => response);
  }
}
