import {Injectable} from '@angular/core';
import {environment} from "../../environments/environment";

/**
 * Manage dev/prod configuration. TODO: To test
 * The script: /script/docker-startup.sh'll overwrite the 'URL' placeholder with the correct backend url
 */
@Injectable()
export class ServerConfigurationService {
  public authServer: string = environment.production ? 'AUTH_URL' : '';
  public ordersServer: string = '';
  public catalogServer: string = '';
}
