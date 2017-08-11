import {Injectable} from '@angular/core';
import {Headers, Http, Response} from "@angular/http";
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import {ServerConfigurationService} from "../server-configuration.service";

@Injectable()
export class AuthenticationService {
  public token: string;

  constructor(private http: Http, private serverConfig: ServerConfigurationService) {
    // set token if saved in local storage
    let currentUser = JSON.parse(localStorage.getItem('currentUser'));
    this.token = currentUser && currentUser.token;
  }

  login(username: string, password: string): Observable<boolean> {

    let headers: Headers = new Headers();
    headers.append('Content-Type', 'application/x-www-form-urlencoded');

    let body = `username=${username}&password=${password}`;

    return this.http.post(this.serverConfig.authServer + '/api/token', body,{headers:headers})
      .map((response: Response) => {
        // login successful if there's a jwt token in the response
        let token = response.json() && response.json().access_token;
        if (token) {
          // set token property
          this.token = token;

          // store username and jwt token in local storage to keep user logged in between page refreshes
          localStorage.setItem('currentUser', JSON.stringify({ username: username, token: token }));

          // return true to indicate successful login
          return true;
        } else {
          // return false to indicate failed login
          return false;
        }
      });
  }

  logout(): void {
    // clear token remove user from local storage to log user out
    this.token = null;
    localStorage.removeItem('currentUser');
  }
}
