
import { CoreModule } from '@abp/ng.core';
import { IdentityModule } from '@abp/ng.identity';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SharedModule } from 'src/app/shared/shared.module';
import { ClientsModule } from '../clients/clients.module';
import { identityEntityActionContributors } from './entity-action-contributors';
import { IdentityExtendedComponent } from './identity-extended.component';

@NgModule({
    imports: [
        CoreModule,
        ThemeSharedModule,
        SharedModule,
        ClientsModule,
        RouterModule.forChild([
            {
                path: '',
                component: IdentityExtendedComponent,
                children: [
                    {
                        path: '',
                        loadChildren: () =>
                            IdentityModule.forLazy({
                                entityActionContributors: identityEntityActionContributors,
                            }),
                    },
                ],
            },
        ]),
    ],
    declarations: [IdentityExtendedComponent]
})
export class IdentityExtendedModule { }
