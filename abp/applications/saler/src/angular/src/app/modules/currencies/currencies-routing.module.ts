import { AuthGuard, PermissionGuard } from '@abp/ng.core';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DailyExchangeListComponent } from './components/daily-exchange-list/daily-exchange-list.component';
import { CurrenciesComponent } from './currencies.component';

const routes: Routes = [
  {
    path: '',
    component: CurrenciesComponent,
    canActivate: [AuthGuard, PermissionGuard]
  },
  {
    path: 'daily-exchanges',
    component: DailyExchangeListComponent,
    canActivate: [AuthGuard, PermissionGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CurrenciesRoutingModule { }
