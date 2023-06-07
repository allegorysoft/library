import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadChildren: () => import('./home/home.module').then(m => m.HomeModule),
  },
  {
    path: 'account',
    loadChildren: () => import('@abp/ng.account').then(m => m.AccountModule.forLazy()),
  },
  {
    path: 'identity',
    loadChildren: () => import('./modules/identity-extended/identity-extended.module')
      .then(m => m.IdentityExtendedModule),
  },
  // {
  //   path: 'tenant-management',
  //   loadChildren: () =>
  //     import('@abp/ng.tenant-management').then(m => m.TenantManagementModule.forLazy()),
  // },
  {
    path: 'setting-management',
    loadChildren: () =>
      import('@abp/ng.setting-management').then(m => m.SettingManagementModule.forLazy()),
  },
  {
    path: 'units',
    loadChildren: () => import('src/app/modules/units/units.module').then(m => m.UnitsModule)
  },
  {
    path: 'items',
    loadChildren: () => import('src/app/modules/items/items.module').then(m => m.ItemsModule)
  },
  {
    path: 'services',
    loadChildren: () =>
      import('src/app/modules/services/services.module').then(m => m.ServicesModule)
  },
  {
    path: 'unit-prices',
    loadChildren: () =>
      import('src/app/modules/unit-prices/unit-prices.module').then(m => m.UnitPricesModule)
  },
  {
    path: 'clients',
    loadChildren: () =>
      import('src/app/modules/clients/clients.module').then(m => m.ClientsModule)
  },
  {
    path: 'orders',
    loadChildren: () => import('src/app/modules/orders/orders.module').then(m => m.OrdersModule)
  },
  {
    path: 'currencies',
    loadChildren: () =>
      import('src/app/modules/currencies/currencies.module').then(m => m.CurrenciesModule)
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
  exports: [RouterModule],
})
export class AppRoutingModule { }
