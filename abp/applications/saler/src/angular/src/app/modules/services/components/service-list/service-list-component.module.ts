import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CoreModule } from '@abp/ng.core';
import { PermissionPipeModule } from 'src/app/core/pipes/permission.pipe';

import { ServiceCreateDialogComponentModule } from '../service-create-dialog/service-create-dialog-component.module';
import { ServiceEditDialogComponentModule } from '../service-edit-dialog/service-edit-dialog-component.module';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { ToolbarModule } from 'primeng/toolbar';
import { DropdownModule } from 'primeng/dropdown';
import { PaginatorModule } from 'primeng/paginator';

import { ServiceListComponent } from './service-list.component';


@NgModule({
    declarations: [ServiceListComponent],
    imports: [
        CommonModule,

        CoreModule,
        PermissionPipeModule,

        ServiceCreateDialogComponentModule,
        ServiceEditDialogComponentModule,

        ButtonModule,
        ToolbarModule,
        TableModule,
        DropdownModule,
        PaginatorModule
    ],
    exports: [ServiceListComponent]
})
export class ServiceListComponentModule { }
