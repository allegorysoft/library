import { AuthGuard, PermissionGuard } from '@abp/ng.core';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { eProductManagementPolicyNames } from 'src/app/config/enums';
import { UnitPricesComponent } from './unit-prices.component';

const routes: Routes = [
  {
    path: 'item',
    canActivate: [AuthGuard, PermissionGuard],
    component: UnitPricesComponent,
    data: { requiredPolicy: eProductManagementPolicyNames.UnitPrice }
  },
  {
    path: 'service',
    canActivate: [AuthGuard, PermissionGuard],
    component: UnitPricesComponent,
    data: { requiredPolicy: eProductManagementPolicyNames.UnitPrice }
  },
  {
    path: '**',
    pathMatch: 'full',
    redirectTo: '/unit-prices/item'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UnitPricesRoutingModule { }
