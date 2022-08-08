import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AdminLayoutComponent } from './components/admin-layout/admin-layout.component';
import { MyProjectNameAdminComponent } from './components/my-project-name-admin.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: AdminLayoutComponent,
    children: [
      {
        path: '',
        component: MyProjectNameAdminComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class MyProjectNameAdminRoutingModule { }
