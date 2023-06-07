import { ABP, RoutesService } from "@abp/ng.core";

export function configRoutes(routesService: RoutesService) {
    routesService.patch('AbpUiNavigation::Menu:Administration', {
        iconClass: 'pi pi-fw pi-shield'
    } as Partial<ABP.Route>);

    routesService.patch('AbpIdentity::Menu:IdentityManagement', {
        iconClass: 'pi pi-fw pi-id-card'
    } as Partial<ABP.Route>);

    routesService.patch('AbpTenantManagement::Menu:TenantManagement', {
        iconClass: 'pi pi-fw pi-people'
    } as Partial<ABP.Route>);

    routesService.patch('AbpSettingManagement::Settings', {
        iconClass: 'pi pi-fw pi-cog'
    } as Partial<ABP.Route>);
}
