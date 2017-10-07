import {Injectable} from '@angular/core';
import {Http, Headers, RequestOptions, Response} from "@angular/http";
import {AuthenticationService} from "../core/security/authentication.service";
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map'
import {ServerConfigurationService} from "../core/server-configuration.service";

@Injectable()
export class ShopService {

  private http: Http;
  private authService: AuthenticationService;
  private serverConfig: ServerConfigurationService;

  constructor(http: Http, authService: AuthenticationService, serverConfig: ServerConfigurationService) {
    this.http = http;
    this.authService = authService;
    this.serverConfig = serverConfig;
  }

  getProducts() : Observable<any> {
    // add authorization header with jwt token
    let headers = new Headers({ 'Authorization': 'Bearer ' + this.authService.token });
    let options = new RequestOptions({ headers: headers });

    return this.http.get(this.serverConfig.catalogServer + '/api/products', options)
      .map((response: Response) => response.json());
  }
}
