import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from "@angular/common/http";
import {Injectable, Injector} from "@angular/core";
import {Observable} from "rxjs/Observable";
import {AuthenticationService} from "./authentication.service";
import {UtilityService} from "../utils.service";

/**
 * Add jwt in every http request
 */
@Injectable()
export class JwtIntegrationHttpInterceptor implements HttpInterceptor {

  constructor(private inj: Injector) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    const auth = this.inj.get(AuthenticationService);

    if(auth.token) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${auth.token}`,
          RequestId: UtilityService.newGuid()
        }
      });
    }

    return next.handle(req)
  }
}
