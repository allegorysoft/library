import { AuthGuard, PermissionGuard } from '@abp/ng.core';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { eOrderManagementPolicyNames } from 'src/app/config/enums';
import { OrderCreateComponent } from './components/order-create/order-create.component';
import { OrderEditComponent } from './components/order-edit/order-edit.component';
import { OrdersComponent } from './orders.component';
import { OrderEditResolver } from './resolvers/order-edit.resolver';

const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuard, PermissionGuard],
    children: [
      {
        path: ':type/create',
        component: OrderCreateComponent,
        canActivate: [PermissionGuard],
        data: {
          requiredPolicy: eOrderManagementPolicyNames.Create
        }
      },
      {
        path: ':type/edit/:id',
        component: OrderEditComponent,
        canActivate: [PermissionGuard],
        data: {
          requiredPolicy: eOrderManagementPolicyNames.Edit
        },
        resolve: { order: OrderEditResolver }
      }
    ]
  },
  {
    path: ':type',
    component: OrdersComponent,
    canActivate: [AuthGuard, PermissionGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OrdersRoutingModule { }
