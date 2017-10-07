import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot} from "@angular/router";
import {Observable} from "rxjs/Observable";
import {AuthenticationService} from "./authentication.service";

@Injectable()
export class AuthGuardService implements CanActivate {

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot)
    : boolean | Observable<boolean> | Promise<boolean> {

    if(this.authService.tokenCheck()) {
      return true;
    }

    this.router.navigate(['/login']);
    return false;
  }

  private router: Router;
  private authService;

  constructor(router: Router, authService: AuthenticationService) {
    this.router = router;
    this.authService = authService;
  }
}
