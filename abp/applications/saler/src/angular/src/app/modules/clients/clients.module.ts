import { NgModule } from '@angular/core';
import { NgxsModule } from '@ngxs/store';

import { SharedModule } from 'src/app/shared/shared.module';
import { PermissionPipeModule } from 'src/app/core/pipes/permission.pipe';

import { ClientsRoutingModule } from './clients-routing.module';

import { ClientState } from './states';

import { ClientsComponent } from './clients.component';
import { ClientListComponent } from './components/client-list/client-list.component';
import { ClientCreateDialogComponent } from './components/client-create-dialog/client-create-dialog.component';
import { ClientEditDialogComponent } from './components/client-edit-dialog/client-edit-dialog.component';


const declarations = [
  ClientsComponent,
  ClientListComponent,
  ClientCreateDialogComponent,
  ClientEditDialogComponent
];

@NgModule({
  declarations: [...declarations],
  imports: [
    ClientsRoutingModule,
    SharedModule,
    PermissionPipeModule,
    NgxsModule.forFeature([
      ClientState
    ]),
  ],
  exports: [...declarations]
})
export class ClientsModule { }
