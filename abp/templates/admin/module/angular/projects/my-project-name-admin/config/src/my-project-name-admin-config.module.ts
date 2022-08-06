import { ModuleWithProviders, NgModule } from '@angular/core';
import { MY_PROJECT_NAME_ADMIN_ROUTE_PROVIDERS } from './providers/route.provider';

@NgModule()
export class MyProjectNameAdminConfigModule {
  static forRoot(): ModuleWithProviders<MyProjectNameAdminConfigModule> {
    return {
      ngModule: MyProjectNameAdminConfigModule,
      providers: [MY_PROJECT_NAME_ADMIN_ROUTE_PROVIDERS],
    };
  }
}
