import { NgFor } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-stock-update',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './stock-update.component.html',
  styleUrl: './stock-update.component.css'
})
export class StockUpdateComponent {
products: any;
cancelUpdate() {
throw new Error('Method not implemented.');
}
selectedProductId: any;
quantityToAdd: any;
updateStock() {
throw new Error('Method not implemented.');
}

}
