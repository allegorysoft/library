import { PagedResultDto } from "@abp/ng.core";
import { ToasterService } from "@abp/ng.theme.shared";
import { Injectable } from "@angular/core";
import { Action, Selector, State, StateContext, Store } from "@ngxs/store";
import { patch, updateItem } from "@ngxs/store/operators";
import { FilteredPagedAndSortedResultRequestDto } from "@proxy";
import { ClientDto, ClientService } from "@proxy/clients";
import { finalize, tap } from "rxjs/operators";
import { ClearClient, CreateClient, DeleteClient, GetClient, GetClients, UpdateClient, UpdateRequestDto, UpdateSavingValue } from "../actions/client.actions";
import { CLIENT_STATE_TOKEN } from "../tokens";

import * as clientConsts from '../consts';

export class ClientStateModel {
    public clients: ClientDto[];
    public requestDto: FilteredPagedAndSortedResultRequestDto;
    public client: ClientDto;
    public saving: boolean;
    public totalCount: number;
    public error: any;
}


@State<ClientStateModel>({
    name: CLIENT_STATE_TOKEN,
    defaults: {
        clients: [],
        requestDto: {
            maxResultCount: clientConsts.maxResultCount,
            skipCount: 0,
            conditions: null,
            sorting: null
        },
        client: null,
        saving: false,
        totalCount: 0,
        error: null
    } as ClientStateModel
})
@Injectable()
export class ClientState {
    //#region Selectors from state
    @Selector()
    static getClients(state: ClientStateModel): ClientDto[] {
        return state.clients || [];
    }

    @Selector()
    static getClient(state: ClientStateModel): ClientDto {
        return state.client || null;
    }

    @Selector()
    static getRequestDto(state: ClientStateModel): FilteredPagedAndSortedResultRequestDto {
        return state.requestDto;
    }
    //#endregion

    //#region Ctor
    constructor(
        private readonly clientService: ClientService,
        private toasterService: ToasterService,
        private store: Store
    ) { }
    //#endregion

    //#region Actions
    @Action(GetClients)
    loadClients(ctx: StateContext<ClientStateModel>, { payload }: GetClients) {
        return this.clientService.list(payload).pipe(
            tap((response: PagedResultDto<ClientDto>) => {
                const state = ctx.getState();
                ctx.setState({
                    ...state,
                    clients: response.items ?? [],
                    totalCount: response.totalCount
                })
            })
        );
    }

    @Action(GetClient)
    loadClient(ctx: StateContext<ClientStateModel>, { payload }: GetClient) {
        return this.clientService.get(payload).pipe(
            tap((response: ClientDto) => {
                const state = ctx.getState();
                ctx.setState({
                    ...state,
                    client: response
                })
            })
        );
    }

    @Action(UpdateRequestDto)
    updateRequestDto(ctx: StateContext<ClientStateModel>, { payload }: UpdateRequestDto) {
        ctx.setState(patch({ requestDto: payload }));
    }

    @Action(CreateClient)
    createClient(ctx: StateContext<ClientStateModel>, { payload }: CreateClient) {
        ctx.setState(patch({ saving: true }))
        return this.clientService.create(payload).pipe(
            tap((client: ClientDto) => {
                const state = ctx.getState();
                this.store.dispatch(new GetClients({ ...state.requestDto }));
                this.toasterService.success('::CreatedSuccessfullyMessage', '::SuccessTitleMessage');
            }),
            finalize(() => ctx.setState(patch({ saving: false })))
        );
    }

    @Action(UpdateClient)
    updateClient(ctx: StateContext<ClientStateModel>, { id, payload }: UpdateClient) {
        ctx.setState(patch({ saving: true }))
        return this.clientService.update(id, payload).pipe(
            tap((response: ClientDto) => {
                ctx.setState(patch({
                    clients: updateItem<ClientDto>(f => f.id === response.id, { ...response })
                }));
                this.toasterService.success('::UpdatedSuccessfullyMessage', '::SuccessTitleMessage');
            }),
            finalize(() => ctx.setState(patch({ saving: false })))
        );
    }

    @Action(DeleteClient)
    deleteClient(ctx: StateContext<ClientStateModel>, { payload }: DeleteClient) {
        return this.clientService.delete(payload).pipe(
            tap(() => {
                this.toasterService.success('::DeletedSuccessfullyMessage', '::SuccessTitleMessage')
                const state = ctx.getState();
                ctx.patchState({
                    ...state,
                    clients: state.clients.filter(client => client.id !== payload),
                    totalCount: state.totalCount - 1
                });
            })
        );
    }

    @Action(UpdateSavingValue)
    updateSavingValue(ctx: StateContext<ClientStateModel>, { payload }: UpdateSavingValue) {
        ctx.setState(patch({ saving: payload }));
    }

    @Action(ClearClient)
    clearClient(ctx: StateContext<ClientStateModel>) {
        ctx.setState(patch({ client: null }));
    }
    //#endregion
}
