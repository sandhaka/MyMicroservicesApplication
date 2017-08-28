import {Injectable} from '@angular/core';
import {Headers, Http, RequestOptions, Response} from "@angular/http";
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import {ServerConfigurationService} from "../server-configuration.service";
import {UtilityService} from "../utils.service";

@Injectable()
export class AuthenticationService {
  public token: string;

  constructor(private http: Http, private serverConfig: ServerConfigurationService) {
    // set token if saved in local storage
    let currentUser = JSON.parse(localStorage.getItem('currentUser'));
    this.token = currentUser && currentUser.token;
  }

  /**
   * Login
   * @param {string} username
   * @param {string} password
   * @returns {Observable<boolean>}
   */
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

          let tokenData = UtilityService.decodeToken(this.token);

          // store username and jwt token in local storage to keep user logged in between page refreshes
          let userData = JSON.stringify(
            {
              username: username,
              userId: tokenData.userId,
              token: token,
              exp: Date.now() + tokenData.exp
            });
          localStorage.setItem('currentUser', userData);

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

  /**
   * Establish if the token is valid, if it's going to expire, renew it
   * @returns {boolean}
   */
  tokenCheck() : boolean {

    if(this.token) {

      let tokenData = JSON.parse(localStorage.getItem('currentUser'));

      // If the token is going to expire in less of a day renew it
      if(tokenData.exp > Date.now() &&
        tokenData.exp < Date.now() + 86400) {

        this.tokenRenew().subscribe((result) => {
          if(!result) {
            console.warn("Error on token renew");
          }
          console.debug("Token renewed");
        });

        return true;
      }
      else if(tokenData.exp < Date.now()) {
        console.debug("Token expired");
        localStorage.removeItem('currentUser');
        return false;
      }

      return true;
    }

    return false;
  }

  /**
   * Renew the current token
   * @returns {Observable<boolean>}
   */
  private tokenRenew() : Observable<boolean> {

    let headers = new Headers({ 'Authorization': 'Bearer ' + this.token });
    let options = new RequestOptions({ headers: headers });

    return this.http.get(this.serverConfig.authServer + '/api/tokenrenew', options)
      .map((response: Response) => {

        let token = response.json() && response.json().access_token;

        if (token) {
          // set token property
          this.token = token;

          let tokenData = UtilityService.decodeToken(this.token);

          // store username and jwt token in local storage to keep user logged in between page refreshes
          let userData = JSON.stringify(
            {
              username: tokenData.username,
              userId: tokenData.userId,
              token: token,
              exp: Date.now() + tokenData.exp
            });
          localStorage.setItem('currentUser', userData);

          return true;
        }

        return false;
    });
  }
}
