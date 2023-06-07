import { NgModule } from '@angular/core';
import { NgxsModule } from '@ngxs/store';
import { UnitsRoutingModule } from './units-routing.module';

import { SharedModule } from 'src/app/shared/shared.module';
import { PermissionPipeModule } from 'src/app/core/pipes/permission.pipe';

import { UnitState } from './states';

import { UnitsComponent } from './units.component';
import { UnitGroupListComponent } from './components/unit-group-list/unit-group-list.component';
import { UnitGroupCreateDialogComponent } from './components/unit-group-create-dialog/unit-group-create-dialog.component';
import { UnitGroupEditDialogComponent } from './components/unit-group-edit-dialog/unit-group-edit-dialog.component';
import { UnitListComponent } from './components/unit-list/unit-list.component';
import { GlobalUnitListComponent } from './components/global-unit-list/global-unit-list.component';

const declarations = [
  UnitsComponent,
  UnitGroupListComponent,
  UnitGroupCreateDialogComponent,
  UnitGroupEditDialogComponent,
  UnitListComponent,
  GlobalUnitListComponent
];


@NgModule({
  declarations: [...declarations],
  imports: [
    UnitsRoutingModule,
    NgxsModule.forFeature([
      UnitState
    ]),
    SharedModule,
    PermissionPipeModule
  ],
  exports: [...declarations]
})
export class UnitsModule { }
