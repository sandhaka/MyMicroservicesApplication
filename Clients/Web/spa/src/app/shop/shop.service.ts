import {Injectable} from '@angular/core';
import {Http, Headers, RequestOptions, Response} from "@angular/http";
import {AuthenticationService} from "../core/security/authentication.service";
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map'
import {Product} from "../shared/product";

@Injectable()
export class ShopService {

  private http: Http;
  private authService: AuthenticationService;

  constructor(http: Http, authService: AuthenticationService) {
    this.http = http;
    this.authService = authService;
  }

  getProducts() : Observable<Product[]> {
    // add authorization header with jwt token
    let headers = new Headers({ 'Authorization': 'Bearer ' + this.authService.token });
    let options = new RequestOptions({ headers: headers });

    return this.http.get('', options)
      .map((response: Response) => response.json());
  }
}
