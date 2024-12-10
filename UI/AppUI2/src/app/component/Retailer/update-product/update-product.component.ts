import { Component, inject, OnInit } from '@angular/core';
import { ProductService } from '../../../services/product.service';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-update-product',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './update-product.component.html',
  styleUrl: './update-product.component.css'
})
export class UpdateProductComponent implements OnInit {

  productService = inject(ProductService);
  router = inject(Router);

  product = {
    productName: '',
    description: '',
    image: null as File | null,
    price: 0,
    stockQuantity: 0,
    sellerId: sessionStorage.getItem('userId') || '',
  };
  imageData: string = '';
  prodId: string = '';

  ngOnInit(): void {
    this.prodId = this.productService.updateProdId;
    if (this.prodId) {
      this.loadData(this.prodId);
    } else {
      console.error('Product ID is not provided');
    }
  }

  loadData(Id: string): void {
    this.productService.getProductById(Id).subscribe(
      (res: any) => {
        this.product.productName = res.data.productName || '';
        this.product.description = res.data.description || '';
        this.product.price = res.data.price || 0;
        this.product.stockQuantity = res.data.stockQuantity || 0;
        this.product.sellerId = res.data.sellerId || this.product.sellerId;
        this.imageData = res.data.imageContent; // Store base64 image data
      },
      (error) => {
        console.error('Error fetching product details:', error);
      }
    );
  }

  onFileChange(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.product.image = file; // Store the selected file
    }
  }

  addStock(){
    this.router.navigateByUrl("layout/stock-update")
  }

  onUpdate(): void {

    if(this.product.price < 0){
      alert("Price Must Be Greater Than 0");
      return;
    }

    const formData = new FormData();
    formData.append('productName', this.product.productName);
    formData.append('price', this.product.price.toString());
    formData.append('stockQuantity', this.product.stockQuantity.toString());
    formData.append('description', this.product.description || '');

    if (this.product.image) {
      formData.append('file', this.product.image, this.product.image.name);
      this.submitForm(formData);
    } else if (this.imageData) {
      this.base64ToFile(this.imageData, 'product-image.jpg')
        .then((file) => {
          formData.append('file', file);
          this.submitForm(formData);
        })
        .catch((error) => {
          console.error('Error converting image data to file:', error);
        });
    } else {
      console.error('No image data available for submission.');
    }
  }

  submitForm(formData: FormData): void {
    this.productService.updateProduct(this.prodId, formData).subscribe(
      (res: any) => {
        console.log('Product updated successfully:', res);
        this.router.navigateByUrl('layout/product-list').then(() => {
          console.log('Navigated to product list.');
        });
      },
      (error) => {
        console.error('Error updating product:', error);
      }
    );
  }

  base64ToFile(base64: string, fileName: string): Promise<File> {
    return fetch(base64)
      .then((res) => res.blob())
      .then((blob) => new File([blob], fileName, { type: blob.type }))
  }

}
