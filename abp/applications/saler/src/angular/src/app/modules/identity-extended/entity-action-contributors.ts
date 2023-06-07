
import {
    eIdentityComponents,
    IdentityEntityActionContributors,
} from '@abp/ng.identity';
import { IdentityUserDto } from '@abp/ng.identity/proxy'
import { EntityAction, EntityActionList } from '@abp/ng.theme.shared/extensions';
import { IdentityExtendedComponent } from './identity-extended.component';

const quickViewAction = new EntityAction<IdentityUserDto>({
    text: '::Actions:SelectClient',
    action: data => {
        const component = data.getInjected(IdentityExtendedComponent);
        component.openClientList(data.record);
    }
});

export function customModalContributor(actionList: EntityActionList<IdentityUserDto>) {
    actionList.addTail(quickViewAction);
}

export const identityEntityActionContributors: IdentityEntityActionContributors = {
    // enum indicates the page to add contributors to
    [eIdentityComponents.Users]: [
        customModalContributor,
        // You can add more contributors here
    ],
};