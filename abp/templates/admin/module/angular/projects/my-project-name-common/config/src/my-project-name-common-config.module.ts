import { ModuleWithProviders, NgModule } from '@angular/core';
import { MY_PROJECT_NAME_COMMON_ROUTE_PROVIDERS } from './providers/route.provider';

@NgModule()
export class MyProjectNameCommonConfigModule {
  static forRoot(): ModuleWithProviders<MyProjectNameCommonConfigModule> {
    return {
      ngModule: MyProjectNameCommonConfigModule,
      providers: [MY_PROJECT_NAME_COMMON_ROUTE_PROVIDERS],
    };
  }
}
