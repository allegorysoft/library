import { RoutesService, eLayoutType } from '@abp/ng.core';
import { APP_INITIALIZER } from '@angular/core';
import {
  eClientManagementPolicyNames,
  eClientManagementRouteNames,
  eGeneralPolicyNames,
  eGeneralRouteNames,
  eOrderManagementPolicyNames,
  eOrderManagementRouteNames,
  eProductManagementPolicyNames,
  eProductManagementRouteNames,
  eTransactionsRouteNames
} from './config/enums';
import { configRoutes } from './config/routes.config';

export const APP_ROUTE_PROVIDER = [
  { provide: APP_INITIALIZER, useFactory: configureRoutes, deps: [RoutesService], multi: true },
];

function configureRoutes(routesService: RoutesService) {
  return () => {
    configRoutes(routesService);

    routesService.add([
      {
        path: '/',
        name: '::Menu:Home',
        iconClass: 'pi pi-fw pi-home',
        order: 1,
        layout: eLayoutType.application,
      },
      {
        name: eProductManagementRouteNames.ProductManagement,
        iconClass: 'pi pi-fw pi-shopping-cart',
        order: 2,
        layout: eLayoutType.application
      },
      {
        path: '/units',
        name: eProductManagementRouteNames.UnitGroup,
        parentName: eProductManagementRouteNames.ProductManagement,
        requiredPolicy: eProductManagementPolicyNames.UnitGroup,
        iconClass: 'pi pi-fw pi-ticket',
        order: 1,
        layout: eLayoutType.application
      },
      {
        path: '/items',
        name: eProductManagementRouteNames.Items,
        parentName: eProductManagementRouteNames.ProductManagement,
        requiredPolicy: eProductManagementPolicyNames.Item,
        iconClass: 'pi pi-fw pi-box',
        order: 2,
        layout: eLayoutType.application
      },
      {
        path: '/services',
        name: eProductManagementRouteNames.Services,
        parentName: eProductManagementRouteNames.ProductManagement,
        requiredPolicy: eProductManagementPolicyNames.Service,
        iconClass: 'pi pi-fw pi-cog',
        order: 3,
        layout: eLayoutType.application
      },
      {
        name: eProductManagementRouteNames.UnitPrices,
        parentName: eProductManagementRouteNames.ProductManagement,
        requiredPolicy: eProductManagementPolicyNames.UnitPrice,
        iconClass: 'pi pi-fw pi-money-bill',
        order: 4,
        layout: eLayoutType.application
      },
      {
        path: '/unit-prices/item',
        name: eProductManagementRouteNames.Item,
        parentName: eProductManagementRouteNames.UnitPrices,
        requiredPolicy: eProductManagementPolicyNames.UnitPrice,
        order: 1,
        layout: eLayoutType.application
      },
      {
        path: '/unit-prices/service',
        name: eProductManagementRouteNames.Service,
        parentName: eProductManagementRouteNames.UnitPrices,
        requiredPolicy: eProductManagementPolicyNames.UnitPrice,
        order: 2,
        layout: eLayoutType.application
      },
      {
        name: eClientManagementRouteNames.ClientManagement,
        iconClass: 'pi pi-fw pi-users',
        order: 3,
        layout: eLayoutType.application
      },
      {
        path: '/clients',
        name: eClientManagementRouteNames.Clients,
        parentName: eClientManagementRouteNames.ClientManagement,
        requiredPolicy: eClientManagementPolicyNames.Clients,
        iconClass: 'pi pi-fw pi-users',
        order: 1,
        layout: eLayoutType.application
      },
      {
        name: eTransactionsRouteNames.Transactions,
        iconClass: 'pi pi-fw pi-sync',
        order: 4,
        layout: eLayoutType.application
      },
      {
        path: undefined,
        name: eOrderManagementRouteNames.Orders,
        parentName: eTransactionsRouteNames.Transactions,
        requiredPolicy: eOrderManagementPolicyNames.Order,
        iconClass: 'pi pi-fw pi-shopping-bag',
        order: 1,
        layout: eLayoutType.application
      },
      {
        path: '/orders/purchase',
        name: eOrderManagementRouteNames.Purchasing,
        parentName: eOrderManagementRouteNames.Orders,
        iconClass: 'pi pi-fw pi-download',
        order: 1,
        layout: eLayoutType.application
      },
      {
        path: '/orders/sales',
        name: eOrderManagementRouteNames.Sales,
        parentName: eOrderManagementRouteNames.Orders,
        iconClass: 'pi pi-fw pi-upload',
        order: 2,
        layout: eLayoutType.application
      },
      {
        name: eGeneralRouteNames.General,
        iconClass: 'pi pi-fw pi-folder',
        order: 5,
        layout: eLayoutType.application
      },
      {
        name: eGeneralRouteNames.Currency,
        parentName: eGeneralRouteNames.General,
        requiredPolicy: eGeneralPolicyNames.Currency,
        iconClass: 'pi pi-fw pi-dollar',
        order: 1,
        layout: eLayoutType.application
      },
      {
        path: '/currencies',
        name: eGeneralRouteNames.Currencies,
        parentName: eGeneralRouteNames.Currency,
        requiredPolicy: eGeneralPolicyNames.Currency,
        iconClass: 'pi pi-fw pi-dollar',
        order: 1,
        layout: eLayoutType.application
      },
      {
        path: '/currencies/daily-exchanges',
        name: eGeneralRouteNames.DailyExchanges,
        parentName: eGeneralRouteNames.Currency,
        requiredPolicy: eGeneralPolicyNames.Currency,
        iconClass: 'pi pi-fw pi-sort-alt',
        order: 2,
        layout: eLayoutType.application
      }
    ]);
  };
}
