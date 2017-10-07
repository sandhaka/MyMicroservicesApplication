import {NgModule} from '@angular/core';
import {CommonModule} from "@angular/common";
import {ReactiveFormsModule} from "@angular/forms";
import {LoginComponent} from "./login.component";
import {CoreModule} from "../core/core.module";

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    CoreModule
  ],
  declarations: [
    LoginComponent
  ],
  exports: [
    LoginComponent
  ],
  providers: [

  ]
})
export class LoginModule {}
