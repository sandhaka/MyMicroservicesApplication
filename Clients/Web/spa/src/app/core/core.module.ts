import {NgModule} from "@angular/core";
import {HttpModule} from "@angular/http";
import {StartupService} from "./startup.service";
import {AuthGuardService} from "./security/auth.guard.service";
import {AuthenticationService} from "./security/authentication.service";
import {ServerConfigurationService} from "./server-configuration.service";

@NgModule({
  imports: [
    HttpModule
  ],
  declarations: [

  ],
  exports: [

  ],
  providers: [
    StartupService,
    AuthenticationService,
    AuthGuardService,
    ServerConfigurationService
  ]
})
export class CoreModule {}
