import { NgModule, NgModuleFactory, ModuleWithProviders } from '@angular/core';
import { CoreModule, LazyModuleFactory } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { MyProjectNameCommonComponent } from './components/my-project-name-common.component';

@NgModule({
  declarations: [MyProjectNameCommonComponent],
  imports: [CoreModule, ThemeSharedModule],
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
