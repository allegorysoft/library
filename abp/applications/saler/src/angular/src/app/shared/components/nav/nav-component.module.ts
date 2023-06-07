import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CoreModule } from '@abp/ng.core';
import { PermissionPipeModule } from 'src/app/core/pipes/permission.pipe';


import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { ToolbarModule } from 'primeng/toolbar';
import { DropdownModule } from 'primeng/dropdown';
import { PaginatorModule } from 'primeng/paginator';
import { MenubarModule } from 'primeng/menubar';
import { MenuModule } from 'primeng/menu';

import { NavComponent } from './nav.component';


@NgModule({
    declarations: [NavComponent],
    imports: [
        CommonModule,
        CoreModule,
        PermissionPipeModule,
        ButtonModule,
        ToolbarModule,
        TableModule,
        DropdownModule,
        PaginatorModule,
        MenubarModule,
        MenuModule
    ],
    exports: [NavComponent]
})
export class NavComponentModule { }
