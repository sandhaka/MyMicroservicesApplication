import {Injectable} from '@angular/core';
import {RequestOptions, Headers} from "@angular/http";

@Injectable()
export class UtilityService {

  /**
   * Decode Json web token content
   * @param {string} token
   * @returns {JSON}
   */
  static decodeToken(token: string) : any {

    let base64 = token.split('.')[1]
      .replace('-', '+')
      .replace('_', '/');

    return JSON.parse(window.atob(base64));
  }

  /**
   * Create default request options for the application
   * @param token
   * @returns {RequestOptions}
   */
  static getRequestOptions(token: string) : RequestOptions {
    // add authorization header with jwt token
    let headers = new Headers({ 'Authorization': 'Bearer ' + token });
    return new RequestOptions({ headers: headers });
  }
}
