import {Injectable} from '@angular/core';
import {AuthenticationService} from "../core/security/authentication.service";
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map'
import {environment} from "../../environments/environment";
import {HttpClient} from "@angular/common/http";

@Injectable()
export class ShopService {

  private http: HttpClient;
  private authService: AuthenticationService;

  constructor(http: HttpClient, authService: AuthenticationService) {
    this.http = http;
    this.authService = authService;
  }

  getProducts() : Observable<any> {
    return this.http.get(environment.settings.catalog_gateway+ '/api/products');
  }
}
