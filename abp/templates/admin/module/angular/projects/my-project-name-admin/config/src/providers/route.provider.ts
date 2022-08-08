import { eLayoutType, RoutesService } from '@abp/ng.core';
import { APP_INITIALIZER } from '@angular/core';
import { eMyProjectNameAdminRouteNames } from '../enums/route-names';

export const MY_PROJECT_NAME_ADMIN_ROUTE_PROVIDERS = [
  {
    provide: APP_INITIALIZER,
    useFactory: configureRoutes,
    deps: [RoutesService],
    multi: true,
  },
];

export function configureRoutes(routesService: RoutesService) {
  return () => {
    routesService.add([
      {
        path: '/my-project-name/admin',
        name: eMyProjectNameAdminRouteNames.MyProjectNameAdmin,
        iconClass: 'fas fa-book',
        layout: eLayoutType.empty,
        order: 4,
      },
    ]);
  };
}
