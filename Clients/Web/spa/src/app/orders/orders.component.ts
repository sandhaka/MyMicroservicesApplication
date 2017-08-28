import {Component, OnInit} from '@angular/core';
import {CreateOrderCommand} from "./order-command";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {Router} from "@angular/router";
import {AuthenticationService} from "../core/security/authentication.service";
import {OrdersService} from "./orders.service";
import {Response} from "@angular/http";
import {BasketService} from "../basket/basket.service";

@Component({
  selector: 'orders',
  templateUrl: 'orders.component.html'
})

export class OrdersComponent implements OnInit {

  form: FormGroup;
  cardExpMonth: number;
  cardExpYear: number;
  paymentError: string;
  orderSubmitted: boolean;

  private formBuilder: FormBuilder;
  private router: Router;
  private authService: AuthenticationService;
  private ordersService: OrdersService;
  private basketService: BasketService;

  orderCommandDto: CreateOrderCommand;

  formErrors = {
    'cardNumber': '',
    'cardHolderName': '',
    'cardSecurityNumber': ''
  };

  validationMessages = {
    'cardNumber': {
      'required': 'Required',
      'minlength': 'Minimum length 12',
      'maxlength': 'Maximum length 19',
      'pattern': 'Must be a sequence of digits'
    },
    'cardHolderName': {
      'required': 'Required'
    },
    'cardSecurityNumber': {
      'required': 'Required',
      'pattern': 'Must be a sequence of 3 digits'
    }
  };

  months = [1,2,3,4,5,6,7,8,9,10,11,12];
  years = [];

  constructor(
    router: Router,
    authService: AuthenticationService,
    formBuilder: FormBuilder,
    ordersService: OrdersService,
    basketService: BasketService) {
    this.orderCommandDto = new CreateOrderCommand();
    this.orderSubmitted = false;

    this.router = router;
    this.authService = authService;
    this.formBuilder = formBuilder;
    this.ordersService = ordersService;
    this.basketService = basketService;
  }

  ngOnInit() {
    this.buildForm();

    // Get the current basket items
    this.basketService.getBasket().subscribe((response) => {
      let basketData = response.result;
      basketData.basketItems.forEach(item => {
        this.orderCommandDto.addItem(
          item.productId, item.productName, item.unitPrice, item.units
        );
      });
    });

    // Setup
    let startYear = new Date(Date.now()).getFullYear();
    for(let i = startYear; i < startYear+10; i++) {
      this.years.push(i);
    }

    this.cardExpMonth = this.months[0];
    this.cardExpYear = this.years[0];
  }

  private buildForm() {
    this.form = this.formBuilder.group({
      'cardNumber': [
        this.orderCommandDto.cardNumber,
        [
          Validators.required,
          Validators.minLength(12),
          Validators.maxLength(19),
          Validators.pattern("[0-9]*")
        ]
      ],
      'cardHolderName': [
        this.orderCommandDto.cardHolderName,
        [
          Validators.required
        ]
      ],
      'cardExpirationMonth': this.cardExpMonth,
      'cardExpirationYear': this.cardExpYear,
      "cardSecurityNumber": [
        this.orderCommandDto.cardSecurityNumber,
        [
          Validators.required,
          Validators.pattern("[0-9]{3}")
        ]
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

  onSubmit() {
    this.paymentError = "";
    let formData = this.form.value;

    this.orderCommandDto.setCardExpiration(formData.cardExpirationMonth, formData.cardExpirationYear);
    this.orderCommandDto.cardNumber = formData.cardNumber;
    this.orderCommandDto.cardHolderName = formData.cardHolderName;
    this.orderCommandDto.cardSecurityNumber = formData.cardSecurityNumber;

    this.ordersService.placeOrder(this.orderCommandDto).subscribe((response: Response) => {
        if(response.ok) {
          this.orderSubmitted = true;
        }
        else {
          this.paymentError = response.statusText;
        }
      },
      (error) => {
        this.paymentError = `Error. Code: ${error.status}`;
        console.error(error);
      });
  }

  goBack() {
    this.router.navigate(['/shop']);
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  getTotalPrice() : number {
    let total = 0;
    for(let i = 0; i < this.orderCommandDto.orderItems.length; i++) {
      total += this.orderCommandDto.orderItems[i].units * this.orderCommandDto.orderItems[i].unitPrice;
    }
    return total;
  }
}
