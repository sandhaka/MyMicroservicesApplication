import {NgModule} from "@angular/core";
import {HttpModule} from "@angular/http";
import {AuthGuardService} from "./security/auth.guard.service";
import {AuthenticationService} from "./security/authentication.service";
import {UtilityService} from "./utils.service";

@NgModule({
  imports: [
    HttpModule
  ],
  declarations: [
  ],
  exports: [
  ],
  providers: [
    AuthenticationService,
    AuthGuardService,
    UtilityService
  ]
})
export class CoreModule {}
