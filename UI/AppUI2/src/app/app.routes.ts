import { Routes } from '@angular/router';
import { LayoutComponent } from './component/Common/layout/layout.component';
import { LoginComponent } from './component/Auth/login/login.component';
import { RegisterComponent } from './component/Auth/register/register.component';
import { HomeComponent } from './component/Customer/home/home.component';
import { AuditComponent } from './component/Admin/audit/audit.component';
import { CartComponent } from './component/Customer/cart/cart.component';
import { ManageUserComponent } from './component/Admin/manage-user/manage-user.component';
import { ProfileComponent } from './component/Customer/profile/profile.component';
import { AddProductComponent } from './component/Retailer/add-product/add-product.component';
import { ProductListComponent } from './component/Retailer/product-list/product-list.component';
import { UpdateProductComponent } from './component/Retailer/update-product/update-product.component';
import { authGuard } from './guard/auth.guard';
import { RetailerStatsComponent } from './component/Retailer/retailer-stats/retailer-stats.component';
import { NotificationComponent } from './component/Common/notification/notification.component';
import { ManageAllProductsComponent } from './component/Admin/manage-all-products/manage-all-products.component';
import { ManageUserRolesComponent } from './component/Admin/manage-user-roles/manage-user-roles.component';
import { StockUpdateComponent } from './component/Retailer/stock-update/stock-update.component';

export const routes: Routes = [
  {
    path: '',
    component: LoginComponent,
  },
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'register',
    component: RegisterComponent,
  },
  {
    path: 'layout',
    component: LayoutComponent,
    // canActivate: [authGuard],
    children: [
      {
        path: 'login',
        component: LoginComponent,
      },
      {
        path: 'register',
        component: RegisterComponent,
      },
      {
        path: 'notification',component: NotificationComponent
      }
    ],
  },
  {
    path: 'layout',
    component: LayoutComponent,
    // canActivate: [authGuard],
    children: [
      {
        path: 'home',
        component: HomeComponent,
      },
      {
        path: 'cart',
        component: CartComponent,
      },
      {
        path: 'profile',
        component: ProfileComponent,
      },
    ],
  },
  {
    path: 'layout',
    component: LayoutComponent,
    // canActivate: [authGuard],
    children: [
      {
        path: 'audit',
        component: AuditComponent,
      },
      {
        path: 'manage-user',
        component: ManageUserComponent,
      },
      {
        path: 'manage-all-products',
        component: ManageAllProductsComponent
      },
      {
        path: 'manage-user-roles',
        component: ManageUserRolesComponent
      }
    ],
  },
  {
    path: 'layout',
    component: LayoutComponent,
    // canActivate: [authGuard],
    children: [
      {
        path: 'add-product',
        component: AddProductComponent,
      },
      {
        path: 'product-list',
        component: ProductListComponent,
      },
      {
        path: 'update-product',
        component: UpdateProductComponent,
      },
      {
        path: 'stats',
        component: RetailerStatsComponent,
      },
      {
        path: 'stock-update',
        component: StockUpdateComponent
      }
    ],
  },
];
