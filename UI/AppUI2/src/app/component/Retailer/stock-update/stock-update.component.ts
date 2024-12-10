import { NgFor } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../../services/product.service';
import { Router } from '@angular/router';
import { AddProductStock } from '../../../model/model';

@Component({
  selector: 'app-stock-update',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './stock-update.component.html',
  styleUrl: './stock-update.component.css',
})
export class StockUpdateComponent {
  products: any;
  productService = inject(ProductService);
  router = inject(Router);
  selectedProductId: string = this.productService.updateProdId;
  quantityToAdd: number = 0;
  cancelUpdate() {
    this.router.navigateByUrl("layout/product-list")
  }
  updateStock() {
    var prod: AddProductStock = new AddProductStock();
    prod.quantityChanged = this.quantityToAdd;
    prod.productId = this.selectedProductId;
    console.log(prod);
    this.productService.addProductStock(prod).subscribe((res:any) =>{
      console.log(res);
      this.router.navigateByUrl("layout/product-list")
    })
  }
}
