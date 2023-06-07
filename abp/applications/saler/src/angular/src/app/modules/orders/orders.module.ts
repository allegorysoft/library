import { NgModule } from '@angular/core';
import { OrdersRoutingModule } from './orders-routing.module';

import { SharedModule } from 'src/app/shared/shared.module';
import { PermissionPipeModule } from 'src/app/core/pipes/permission.pipe';

import { NgxsModule } from '@ngxs/store';

import { ProductCalculationsModule } from '../product-calculations/product-calculations.module';

import { ClientsModule } from '../clients/clients.module';

import { ItemsModule } from '../items/items.module';
import { ItemLookupListComponentModule } from '../items/components/item-lookup-list/item-lookup-list-component.module';

import { ServicesModule } from '../services/services.module';
import { ServiceLookupListComponentModule } from '../services/components/service-lookup-list/service-lookup-list-component.module';

import { CurrenciesModule } from '../currencies/currencies.module';

import { OrderState } from './states';


import { OrdersComponent } from './orders.component';
import { OrderListComponent } from './components/order-list/order-list.component';
import { OrderLineComponent } from './components/order-line/order-line.component';
import { OrderCreateComponent } from './components/order-create/order-create.component';
import { OrderEditComponent } from './components/order-edit/order-edit.component';
import { DiscountCalculateComponent } from './components/discount-calculate/discount-calculate.component';
import { DiscountAggregateCalculateComponent } from './components/discount-aggregate-calculate/discount-aggregate-calculate.component';


const declarations = [
  OrdersComponent,
  OrderListComponent,
  OrderLineComponent,
  OrderCreateComponent,
  OrderEditComponent,
  DiscountCalculateComponent,
  DiscountAggregateCalculateComponent
];

@NgModule({
  declarations: [...declarations],
  imports: [
    OrdersRoutingModule,

    SharedModule,
    PermissionPipeModule,
    NgxsModule.forFeature([
      OrderState
    ]),
    ProductCalculationsModule,

    ClientsModule,

    ItemsModule,
    ItemLookupListComponentModule,

    ServicesModule,
    ServiceLookupListComponentModule,

    CurrenciesModule
  ],
  exports: [...declarations]
})
export class OrdersModule { }
