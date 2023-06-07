import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AuthGuard, PermissionGuard } from '@abp/ng.core';

import { UnitsComponent } from './units.component';
import { UnitGroupResolver } from './resolvers/unit-group.resolver';


const routes: Routes = [
  {
    path: '',
    component: UnitsComponent,
    canActivate: [AuthGuard, PermissionGuard]
    // resolve: { unitGroups: UnitGroupResolver }
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UnitsRoutingModule { }
