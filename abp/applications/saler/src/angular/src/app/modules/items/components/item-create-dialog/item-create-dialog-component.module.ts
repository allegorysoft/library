import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CoreModule } from '@abp/ng.core';
import { PermissionPipeModule } from 'src/app/core/pipes/permission.pipe';

import { UnitsModule } from 'src/app/modules/units/units.module';
import { ProductCalculationsModule } from 'src/app/modules/product-calculations/product-calculations.module';

import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';

import { ItemCreateDialogComponent } from './item-create-dialog.component';


@NgModule({
    declarations: [ItemCreateDialogComponent],
    imports: [
        CommonModule,
        PermissionPipeModule,
        UnitsModule,
        ProductCalculationsModule,
        CoreModule,
        ButtonModule,
        DialogModule,
        InputTextModule,
        InputNumberModule
    ],
    exports: [ItemCreateDialogComponent]
})
export class ItemCreateDialogComponentModule { }
