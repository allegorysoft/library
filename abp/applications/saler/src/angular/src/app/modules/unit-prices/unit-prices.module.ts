import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgxsModule } from '@ngxs/store';
import { UnitPriceState } from './states';

import { UnitPricesRoutingModule } from './unit-prices-routing.module';
import { UnitPricesComponent } from './unit-prices.component';
import { UnitPriceListModule } from './components/unit-price-list/unit-price-list.module';

const declarations = [UnitPricesComponent];

@NgModule({
  declarations: [...declarations],
  imports: [
    CommonModule,
    UnitPricesRoutingModule,
    UnitPriceListModule,
    NgxsModule.forFeature([
      UnitPriceState
    ]),
  ],
  exports: [...declarations]
})
export class UnitPricesModule { }
