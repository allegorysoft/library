import { NgModule, NgModuleFactory, ModuleWithProviders } from '@angular/core';
import { CoreModule, LazyModuleFactory } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { MyProjectNameAdminComponent } from './components/my-project-name-admin.component';
import { MyProjectNameAdminRoutingModule } from './my-project-name-admin-routing.module';

@NgModule({
  declarations: [MyProjectNameAdminComponent],
  imports: [CoreModule, ThemeSharedModule, MyProjectNameAdminRoutingModule],
  exports: [MyProjectNameAdminComponent],
})
export class MyProjectNameAdminModule {
  static forChild(): ModuleWithProviders<MyProjectNameAdminModule> {
    return {
      ngModule: MyProjectNameAdminModule,
      providers: [],
    };
  }

  static forLazy(): NgModuleFactory<MyProjectNameAdminModule> {
    return new LazyModuleFactory(MyProjectNameAdminModule.forChild());
  }
}
