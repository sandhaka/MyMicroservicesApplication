import {ProductData} from "../core/shared/product-data";

export class CreateOrderCommand {
  orderItems: Array<ProductData>;
  cardNumber: string;
  cardHolderName: string;
  cardSecurityNumber: number;
  cardExpiration: Date;

  constructor() {
    this.orderItems = [];
  }

  addItem(productId: number,
          productName: string,
          unitPrice: number,
          units: number) {

    let item = new ProductData();

    item.unitPrice = unitPrice;
    item.units = units;
    item.productName = productName;
    item.productId = productId;

    this.orderItems.push(item);
  }

  setCardExpiration(month: number, year: number) {
    this.cardExpiration = new Date();
    this.cardExpiration.setMonth(month);
    this.cardExpiration.setFullYear(year);
  }
}
