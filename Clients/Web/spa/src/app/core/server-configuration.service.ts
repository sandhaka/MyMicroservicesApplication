import {Injectable} from '@angular/core';
import {environment} from "../../environments/environment";

@Injectable()
export class ServerConfigurationService {
  public authServer: string = environment.production ? 'AUTH_URL' : '';
  public ordersServer: string = '';
  public catalogServer: string = '';
}
