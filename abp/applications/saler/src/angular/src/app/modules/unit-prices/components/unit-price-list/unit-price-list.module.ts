import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';

import { PermissionPipeModule } from 'src/app/core/pipes/permission.pipe';

import { TableModule } from 'primeng/table';
import { SkeletonModule } from 'primeng/skeleton';
import { ToolbarModule } from 'primeng/toolbar';
import { ButtonModule } from 'primeng/button';

import { UnitPriceEditDialogModule } from '../unit-price-edit-dialog/unit-price-edit-dialog.module';

import { UnitPriceListComponent } from './unit-price-list.component';


@NgModule({
  declarations: [
    UnitPriceListComponent
  ],
  imports: [
    CommonModule,
    CoreModule,
    ThemeSharedModule,
    PermissionPipeModule,
    ButtonModule,
    ToolbarModule,
    TableModule,
    SkeletonModule,
    UnitPriceEditDialogModule
  ],
  exports: [UnitPriceListComponent]
})
export class UnitPriceListModule { }
