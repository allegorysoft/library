import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CoreModule } from '@abp/ng.core';
import { PermissionPipeModule } from 'src/app/core/pipes/permission.pipe';

import { ProductCalculationsModule } from 'src/app/modules/product-calculations/product-calculations.module';
import { UnitsModule } from 'src/app/modules/units/units.module';

import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';

import { ServiceCreateDialogComponent } from './service-create-dialog.component';


@NgModule({
    declarations: [ServiceCreateDialogComponent],
    imports: [
        CommonModule,

        CoreModule,
        PermissionPipeModule,

        UnitsModule,
        ProductCalculationsModule,

        ButtonModule,
        DialogModule,
        InputTextModule,
        InputNumberModule
    ],
    exports: [ServiceCreateDialogComponent]
})
export class ServiceCreateDialogComponentModule { }
