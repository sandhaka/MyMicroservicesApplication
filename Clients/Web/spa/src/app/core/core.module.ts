import {NgModule} from "@angular/core";
import {HttpModule} from "@angular/http";
import {AuthGuardService} from "./security/auth.guard.service";
import {AuthenticationService} from "./security/authentication.service";
import {ServerConfigurationService} from "./server-configuration.service";
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
    ServerConfigurationService,
    UtilityService
  ]
})
export class CoreModule {}
