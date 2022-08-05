import { NgModule, NgModuleFactory, ModuleWithProviders } from '@angular/core';
import { CoreModule, LazyModuleFactory } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { MyProjectNameCommonComponent } from './components/my-project-name-common.component';
import { MyProjectNameCommonRoutingModule } from './my-project-name-common-routing.module';

@NgModule({
  declarations: [MyProjectNameCommonComponent],
  imports: [CoreModule, ThemeSharedModule, MyProjectNameCommonRoutingModule],
  exports: [MyProjectNameCommonComponent],
})
export class MyProjectNameCommonModule {
  static forChild(): ModuleWithProviders<MyProjectNameCommonModule> {
    return {
      ngModule: MyProjectNameCommonModule,
      providers: [],
    };
  }

  static forLazy(): NgModuleFactory<MyProjectNameCommonModule> {
    return new LazyModuleFactory(MyProjectNameCommonModule.forChild());
  }
}
