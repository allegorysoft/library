import { eLayoutType, RoutesService } from '@abp/ng.core';
import { APP_INITIALIZER } from '@angular/core';
import { eMyProjectNamePublicRouteNames } from '../enums/route-names';

export const MY_PROJECT_NAME_PUBLIC_ROUTE_PROVIDERS = [
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
        path: '/my-project-name/public',
        name: eMyProjectNamePublicRouteNames.MyProjectNamePublic,
        iconClass: 'fas fa-book',
        layout: eLayoutType.application,
        order: 5,
      },
    ]);
  };
}
