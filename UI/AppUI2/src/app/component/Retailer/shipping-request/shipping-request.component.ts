import { Component, inject } from '@angular/core';
import { Filter, Product } from '../../../model/model';
import { Router } from '@angular/router';
import { ProductService } from '../../../services/product.service';

@Component({
  selector: 'app-shipping-request',
  standalone: true,
  imports: [],
  templateUrl: './shipping-request.component.html',
  styleUrl: './shipping-request.component.css'
})
export class ShippingRequestComponent {
  products: any[] = [];
  router = inject(Router);
  productService = inject(ProductService);
  filter = new Filter();
  currentPage: any;
  totalPages: any;


  ngOnInit(): void {
    this.getPendingOrders();
  }

  shipProduct(Id: string) {
this.productService.shipProduct(Id).subscribe();
  }

  getPendingOrders(){
    this.productService.getPendingOrders().subscribe((res:any)=>{
      this.products = res.data;
      console.log(res.data);
    })
  }

}
