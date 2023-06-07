import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { PermissionPipeModule } from 'src/app/core/pipes/permission.pipe';

import { ItemsModule } from 'src/app/modules/items/items.module';
import { ItemListComponentModule } from 'src/app/modules/items/components/item-list/item-list-component.module';

import { ServicesModule } from 'src/app/modules/services/services.module';
import { ServiceListComponentModule } from 'src/app/modules/services/components/service-list/service-list-component.module';

import { ClientsModule } from 'src/app/modules/clients/clients.module';
import { CurrenciesModule } from 'src/app/modules/currencies/currencies.module';

import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { CheckboxModule } from 'primeng/checkbox';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';

import { UnitPriceEditDialogComponent } from './unit-price-edit-dialog.component';

@NgModule({
  declarations: [UnitPriceEditDialogComponent],
  imports: [
    CommonModule,
    CoreModule,
    ThemeSharedModule,
    PermissionPipeModule,

    ItemsModule,
    ItemListComponentModule,

    ServicesModule,
    ServiceListComponentModule,

    ClientsModule,
    CurrenciesModule,

    ButtonModule,
    DialogModule,
    InputTextModule,
    InputNumberModule,
    CheckboxModule,
    CalendarModule,
    DropdownModule
  ],
  exports: [UnitPriceEditDialogComponent]
})
export class UnitPriceEditDialogModule { }
