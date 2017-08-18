export class ProductData {

  constructor(id: number, productName: string, unitPrice: number, _package: string, assets: number) {
    this.UnitPrice = unitPrice;
    this.ProductName = productName;
    this.Assets = assets;
    this.Id = id;
    this.Package = _package;
  }

  Id: number;
  ProductName: string;
  UnitPrice: number;
  Package: string;
  Assets: number;
}
