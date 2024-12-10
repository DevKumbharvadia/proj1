import { Component, inject, OnInit } from '@angular/core';
import { Product, ProductStockLog } from '../../../model/model';
import { ProductService } from '../../../services/product.service';
import { CurrencyPipe, DatePipe, NgFor, NgIf } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-product-details',
  standalone: true,
  imports: [NgFor, CurrencyPipe, NgIf, DatePipe],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.css',
})
export class ProductDetailsComponent implements OnInit {
  product: Product = new Product();
  stockLogs: ProductStockLog[] = [];
  productServices = inject(ProductService);
  showLogs: boolean = false;
  router = inject(Router);

  ngOnInit(): void {
    this.getStockLogs();
    this.getProductDetails();
  }

  toggleStockLogs() {
    this.showLogs = !this.showLogs;
  }

  getStockLogs() {
    this.productServices.getStockLogs().subscribe((res: any) => {
      this.stockLogs = res.data;
    });
  }

  getProductDetails() {
    this.productServices.getProductDetails().subscribe(
      (res: any) => {
        this.product = res.data;
      },
      (error) => {
        this.router.navigateByUrl("layout/product-list");
      }
    );
  }
}
