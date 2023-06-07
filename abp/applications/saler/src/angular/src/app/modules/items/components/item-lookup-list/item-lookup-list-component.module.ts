import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CoreModule } from '@abp/ng.core';
import { PermissionPipeModule } from 'src/app/core/pipes/permission.pipe';

import { ItemCreateDialogComponentModule } from '../item-create-dialog/item-create-dialog-component.module';
import { ItemEditDialogComponentModule } from '../item-edit-dialog/item-edit-dialog-component.module';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { ToolbarModule } from 'primeng/toolbar';
import { DropdownModule } from 'primeng/dropdown';
import { PaginatorModule } from 'primeng/paginator';

import { ItemLookupListComponent } from './item-lookup-list.component';


@NgModule({
  declarations: [ItemLookupListComponent],
  imports: [
    CommonModule,
    CoreModule,
    PermissionPipeModule,
    ItemCreateDialogComponentModule,
    ItemEditDialogComponentModule,
    ButtonModule,
    ToolbarModule,
    TableModule,
    DropdownModule,
    PaginatorModule
  ],
  exports: [ItemLookupListComponent]
})
export class ItemLookupListComponentModule { }
