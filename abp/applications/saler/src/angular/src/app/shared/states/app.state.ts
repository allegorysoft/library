import { Injectable } from '@angular/core';
import { Action, State, StateContext, Selector } from '@ngxs/store';
import { AppOccurError, ToggleSidebar } from '../actions';

export interface AppStateModel {
    error: any;
    pageSizeOptions: number[];
    sidebarVisible: boolean;
}

const defaults: AppStateModel = {
    error: null,
    pageSizeOptions: [10, 20, 50, 100, 200],
    sidebarVisible: false
};

@State<AppStateModel>({
    name: 'app',
    defaults //Names are matching with local variable, so we don't need to use like 'defaults:defaults'
})
@Injectable({
    providedIn: 'root'
})
export class AppState {
    //#region Selectors from state
    @Selector()
    static getPageSizeOptions(state: AppStateModel): number[] {
        return state.pageSizeOptions;
    }
    //#endregion

    //#region Ctor
    constructor() { }
    //#endregion

    //#region Actions
    @Action(AppOccurError)
    authorOccurError(ctx: StateContext<AppStateModel>, { payload }: AppOccurError) {
        ctx.setState({
            ...ctx.getState(),
            error: payload
        });
    }

    @Action(ToggleSidebar)
    toggleSidebar(ctx: StateContext<AppStateModel>, { payload }: ToggleSidebar) {
        ctx.setState({
            ...ctx.getState(),
            sidebarVisible: payload
        });
    }
    //#endregion
}
