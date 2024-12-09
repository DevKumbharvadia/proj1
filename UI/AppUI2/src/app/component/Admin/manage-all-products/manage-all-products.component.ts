import { Component, inject } from '@angular/core';
import { Filter, Product } from '../../../model/model';
import { Router } from '@angular/router';
import { ProductService } from '../../../services/product.service';

@Component({
  selector: 'app-manage-all-products',
  standalone: true,
  imports: [],
  templateUrl: './manage-all-products.component.html',
  styleUrl: './manage-all-products.component.css'
})
export class ManageAllProductsComponent {
  products: Product[] = [];
  router = inject(Router);
  productService = inject(ProductService);
  filter = new Filter();
currentPage: any;
totalPages: any;

  ngOnInit(): void {
    this.getSortedProducts();
  }

  nextPage() {
    this.filter.Page+=1;
    this.productService.getSortedProducts(this.filter).subscribe((res: any) => {
      this.products = res.data.result;
      this.currentPage = res.data.currentPage;
      this.totalPages = res.data.totalPages;
    });
    }

    previousPage() {
      this.filter.Page-=1;
      this.productService.getSortedProducts(this.filter).subscribe((res: any) => {
        this.products = res.data.result;
        this.currentPage = res.data.currentPage;
        this.totalPages = res.data.totalPages;
      });
      }

      getSortedProducts() {
    this.productService.getSortedProducts(this.filter).subscribe((res: any) => {
      this.products = res.data.result;
      this.currentPage = res.data.currentPage;
      this.totalPages = res.data.totalPages;
    });
  }

  deleteProduct(productId: string): void {
    if (confirm('Are you sure you want to delete this product?')) {
      this.productService.deleteProduct(productId).subscribe((res: any) => {
        alert(res.message);
        this.products = this.products.filter(
          (product) => product.productId !== productId
        );
      });
    }
  }

}
