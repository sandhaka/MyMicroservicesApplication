import {Injectable} from '@angular/core';

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
}
