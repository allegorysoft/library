import { NgModule } from '@angular/core';
import { NgxsModule } from '@ngxs/store';

import { SharedModule } from 'src/app/shared/shared.module';
import { PermissionPipeModule } from 'src/app/core/pipes/permission.pipe';

import { CurrenciesRoutingModule } from './currencies-routing.module';

import { CurrencyState } from './states';

import { CurrenciesComponent } from './currencies.component';
import { CurrencyListComponent } from './components/currency-list/currency-list.component';
import { CurrencyEditDialogComponent } from './components/currency-edit-dialog/currency-edit-dialog.component';
import { DailyExchangeListComponent } from './components/daily-exchange-list/daily-exchange-list.component';

export const declarations = [
  CurrenciesComponent,
  CurrencyListComponent,
  CurrencyEditDialogComponent,
  DailyExchangeListComponent
];

@NgModule({
  declarations: [...declarations],
  imports: [
    CurrenciesRoutingModule,
    SharedModule,
    PermissionPipeModule,
    NgxsModule.forFeature([
      CurrencyState
    ]),
  ],
  exports: [...declarations]
})
export class CurrenciesModule { }
