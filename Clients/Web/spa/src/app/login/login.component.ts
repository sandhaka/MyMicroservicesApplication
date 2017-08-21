
import {Component, OnInit} from "@angular/core";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {Router} from "@angular/router";
import {UserData} from "./user-data";
import {AuthenticationService} from "../core/security/authentication.service";

@Component({
  templateUrl: 'login.component.html',
  styleUrls: ['login.component.css']
})
export class LoginComponent implements OnInit {
  form: FormGroup;
  private formBuilder: FormBuilder;
  private userDto: UserData;
  private router: Router;
  private authService: AuthenticationService;

  formErrors = {
    'username': '',
    'password': ''
  };

  validationMessages = {
    'username': {
      'required': 'Required'
    },
    'password': {
      'required': 'Required'
    }
  };

  loginError = '';

  constructor(formBuilder: FormBuilder, router: Router, authService: AuthenticationService) {
    this.formBuilder = formBuilder;
    this.userDto = new UserData();
    this.router = router;
    this.authService = authService;

    this.buildForm();
  }

  ngOnInit() {
    if(this.authService.tokenCheck()) {
      this.router.navigate(['/shop']);
      return;
    }
  }

  /**
   * Login action
   */
  onNgSubmit() {
    this.userDto = this.form.value;
    this.authService.login(this.userDto.username, this.userDto.password).subscribe(
      (result: any) => {
        if(result) {
          this.router.navigate(['/shop']);
        }
        else {
          this.loginError = 'Username/password not valid';
        }
      }
    );
  }

  /**
   * Setup the login form
   */
  private buildForm() {
    this.form = this.formBuilder.group({
      'username': [
        this.userDto.username,
        Validators.required
      ],
      'password': [
        this.userDto.password,
        Validators.required
      ]
    });

    this.form.valueChanges.subscribe((data) => {
      this.onDataChanged(data);
    });
    this.onDataChanged();
  }

  /**
   * Setup the error messages
   * @param data
   */
  private onDataChanged(data?: any) {
    if (!this.form)
      return;

    const _form = this.form;

    for (const field in this.formErrors) {
      const control = _form.get(field);
      this.formErrors[field] = '';

      if (control && control.dirty && !control.valid) {
        const messages = this.validationMessages[field];
        for (const key in control.errors) {
          this.formErrors[field] += messages[key] + ' ';
        }
      }
    }
  }
}
