import { ModuleWithProviders, NgModule } from '@angular/core';
import { MY_PROJECT_NAME_PUBLIC_ROUTE_PROVIDERS } from './providers/route.provider';

@NgModule()
export class MyProjectNamePublicConfigModule {
  static forRoot(): ModuleWithProviders<MyProjectNamePublicConfigModule> {
    return {
      ngModule: MyProjectNamePublicConfigModule,
      providers: [MY_PROJECT_NAME_PUBLIC_ROUTE_PROVIDERS],
    };
  }
}
