import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PublicLayoutComponent } from './components/public-layout/public-layout.component';
import { MyProjectNamePublicComponent } from './components/my-project-name-public.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: PublicLayoutComponent,
    children: [
      {
        path: '',
        component: MyProjectNamePublicComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class MyProjectNamePublicRoutingModule { }
