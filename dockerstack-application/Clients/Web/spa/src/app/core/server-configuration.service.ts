import {Injectable} from '@angular/core';
import {environment} from "../../environments/environment";

/**
 * Manage dev/prod configuration.
 * overwrite the 'URL' placeholder with the correct backend url after build
 */
@Injectable()
export class ServerConfigurationService {
  public authServer: string = environment.production ? 'AUTH_URL' : '';
  public ordersServer: string = environment.production ? 'ORDERS_URL' : '';
  public catalogServer: string = environment.production ? 'CATALOG_URL' : '';
  public basketServer: string = environment.production ? 'BASKET_URL' : '';
}
