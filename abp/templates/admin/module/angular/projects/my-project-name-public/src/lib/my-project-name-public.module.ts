import { NgModule, NgModuleFactory, ModuleWithProviders } from '@angular/core';
import { CoreModule, LazyModuleFactory } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { MyProjectNamePublicComponent } from './components/my-project-name-public.component';
import { MyProjectNamePublicRoutingModule } from './my-project-name-public-routing.module';

@NgModule({
  declarations: [MyProjectNamePublicComponent],
  imports: [CoreModule, ThemeSharedModule, MyProjectNamePublicRoutingModule],
  exports: [MyProjectNamePublicComponent],
})
export class MyProjectNamePublicModule {
  static forChild(): ModuleWithProviders<MyProjectNamePublicModule> {
    return {
      ngModule: MyProjectNamePublicModule,
      providers: [],
    };
  }

  static forLazy(): NgModuleFactory<MyProjectNamePublicModule> {
    return new LazyModuleFactory(MyProjectNamePublicModule.forChild());
  }
}
