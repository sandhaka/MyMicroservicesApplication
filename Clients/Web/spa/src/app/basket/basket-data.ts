import {ProductData} from "../core/shared/product-data";

export class BasketData {
  identity: string;
  basketItems: ProductData[];

  constructor(identity: string) {
    this.identity = identity;
    this.basketItems = [];
  }

  addItem(productId: number, productName: string, unitPrice: number, units: number) {
    if(this.basketItems.filter((item) => item.productId === productId).length > 0) {
      this.basketItems.filter((item) => item.productId === productId)[0].units += units;
    }
    else {
      this.basketItems.push({
        productId: productId,
        productName: productName,
        units: units,
        unitPrice: unitPrice
      });
    }
  }

  getTotalPrice() : number {
    let total = 0;
    this.basketItems.forEach(item => {
      total += item.units * item.unitPrice;
    });
    return total;
  }

  copyFrom(data: any) {
    this.identity = data.identity;
    this.basketItems = []; // Clear current
    data.basketItems.forEach(item => {
      this.basketItems.push(
        {
          productId: parseInt(item.productId),
          productName: item.productName,
          units: parseInt(item.units),
          unitPrice: parseFloat(item.unitPrice)
        }
      );
    });
  }
}
