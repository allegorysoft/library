import { Injectable } from "@angular/core";
import { finalize, tap } from "rxjs/operators";
import { Action, Selector, State, StateContext, Store } from "@ngxs/store";
import { patch, removeItem } from "@ngxs/store/operators";
import { PagedResultDto } from "@abp/ng.core";
import { ToasterService } from "@abp/ng.theme.shared";
import { FilteredPagedAndSortedResultRequestDto } from "@proxy";
import { GetServiceLookupListDto, ServiceDto, ServiceLookupDto, ServiceService, ServiceWithDetailsDto } from "@proxy/services";
import {
    ClearService,
    CreateService,
    DeleteService,
    GetService,
    GetServices,
    UpdateService,
    UpdateRequestDto,
    GetServiceByCode,
    PatchWorkOnLookup,
    GetLookupServices,
    UpdateLookupRequestDto
} from "../actions";
import { SERVICE_STATE_TOKEN } from "../tokens";
import * as serviceConsts from '../consts';

export class ServiceStateModel {
    public services: ServiceDto[];
    public totalCount: number;
    public loading: boolean;
    public requestDto: FilteredPagedAndSortedResultRequestDto;

    public workOnLookup: boolean;
    public lookupServices: ServiceLookupDto[];
    public lookupTotalCount: number;
    public loadingLookup: boolean;
    public lookupRequestDto: GetServiceLookupListDto;

    public service: ServiceWithDetailsDto;
    public busy: boolean;
}

@State<ServiceStateModel>({
    name: SERVICE_STATE_TOKEN,
    defaults: <ServiceStateModel>{
        services: [],
        totalCount: 0,
        loading: false,
        requestDto: {
            maxResultCount: serviceConsts.maxResultCount,
            skipCount: 0,
            conditions: null,
            sorting: null
        },

        workOnLookup: false,
        lookupServices: [],
        lookupTotalCount: 0,
        loadingLookup: false,
        lookupRequestDto: {
            maxResultCount: serviceConsts.maxResultCount,
            skipCount: 0,
            conditions: null,
            sorting: null,
            date: null,
            isSales: null,
            clientCode: null
        },

        service: null,
        busy: false
    }
})
@Injectable()
export class ServiceState {
    //#region Selectors from state
    @Selector()
    static getServices(state: ServiceStateModel): ServiceDto[] {
        return state.services || [];
    }

    @Selector()
    static getTotalCount(state: ServiceStateModel): number {
        return state.totalCount || 0;
    }

    @Selector()
    static getLoading(state: ServiceStateModel): boolean {
        return state.loading || false;
    }

    @Selector()
    static getLookupServices(state: ServiceStateModel): ServiceLookupDto[] {
        return state.lookupServices || [];
    }

    @Selector()
    static getLookupTotalCount(state: ServiceStateModel): number {
        return state.lookupTotalCount || 0;
    }

    @Selector()
    static getLoadingLookup(state: ServiceStateModel): boolean {
        return state.loadingLookup || false;
    }

    @Selector()
    static getService(state: ServiceStateModel): ServiceWithDetailsDto {
        return state.service || null;
    }
    //#endregion

    //#region Ctor
    constructor(
        private store: Store,
        private readonly service: ServiceService,
        private readonly toasterService: ToasterService
    ) { }
    //#endregion

    //#region Actions
    @Action(GetServices)
    loadServices(ctx: StateContext<ServiceStateModel>, { payload }: GetServices) {
        ctx.setState(patch({ loading: true }));
        return this.service.list(payload).pipe(
            tap(response => {
                ctx.setState(patch({
                    services: response.items,
                    totalCount: response.totalCount
                }))
            }),
            finalize(() => ctx.setState(patch({ loading: false })))
        );
    }

    @Action(UpdateRequestDto)
    updateRequestDto(ctx: StateContext<ServiceStateModel>, { payload }: UpdateRequestDto) {
        ctx.setState(patch({ requestDto: payload }));
    }

    @Action(PatchWorkOnLookup)
    patchWorkOnLookup(ctx: StateContext<ServiceStateModel>, { payload }: PatchWorkOnLookup) {
        ctx.setState(patch({ workOnLookup: payload }));
    }

    @Action(GetLookupServices)
    loadLookupServices(ctx: StateContext<ServiceStateModel>, { payload }: GetLookupServices) {
        ctx.setState(patch({ loadingLookup: true }))
        return this.service.listServiceLookup(payload).pipe(
            tap(response => {
                ctx.setState(
                    patch({
                        lookupServices: response.items,
                        lookupTotalCount: response.totalCount
                    })
                )
            }),
            finalize(() => ctx.setState(patch({ loadingLookup: false })))
        );
    }

    @Action(UpdateLookupRequestDto)
    updateLookupRequestDto(ctx: StateContext<ServiceStateModel>, { payload }: UpdateLookupRequestDto) {
        ctx.setState(patch({ lookupRequestDto: payload }));
    }

    @Action(GetService)
    loadService(ctx: StateContext<ServiceStateModel>, { payload }: GetService) {
        return this.service.get(payload).pipe(
            tap((response: ServiceWithDetailsDto) => {
                ctx.setState(patch({ service: response }));
            })
        );
    }

    @Action(GetServiceByCode)
    loadServiceByCode(ctx: StateContext<ServiceStateModel>, { payload }: GetServiceByCode) {
        return this.service.getByCode(payload).pipe(
            tap((response: ServiceWithDetailsDto) => {
                ctx.setState(patch({ service: response }));
            })
        );
    }

    @Action(CreateService)
    createService(ctx: StateContext<ServiceStateModel>, { payload }: CreateService) {
        ctx.setState(patch({ busy: true }))
        return this.service.create(payload).pipe(
            tap(() => {
                const state = ctx.getState();

                this.store.dispatch(
                    state.workOnLookup
                        ? new GetLookupServices({ ...state.lookupRequestDto })
                        : new GetServices({ ...state.requestDto })
                );

                this.toasterService.success(
                    '::CreatedSuccessfullyMessage', '::SuccessTitleMessage'
                );
            }),
            finalize(() => ctx.setState(patch({ busy: false })))
        );
    }

    @Action(UpdateService)
    updateService(ctx: StateContext<ServiceStateModel>, { id, payload }: UpdateService) {
        ctx.setState(patch({ busy: true }))
        return this.service.update(id, payload).pipe(
            tap(() => {
                const state = ctx.getState();

                this.store.dispatch(
                    state.workOnLookup
                        ? new GetLookupServices({ ...state.lookupRequestDto })
                        : new GetServices({ ...state.requestDto })
                );

                this.toasterService.success(
                    '::UpdatedSuccessfullyMessage', '::SuccessTitleMessage'
                );
            }),
            finalize(() => ctx.setState(patch({ busy: false })))
        );
    }

    @Action(DeleteService)
    deleteService(ctx: StateContext<ServiceStateModel>, { payload }: DeleteService) {
        return this.service.delete(payload).pipe(
            tap(() => {
                this.toasterService.success('::DeletedSuccessfullyMessage', '::SuccessTitleMessage')
                const state = ctx.getState();
                ctx.setState(patch({
                    services: removeItem<ServiceDto>(f => f.id === payload),
                    lookupServices: removeItem<ServiceLookupDto>(f => f.id === payload),
                    totalCount: state.totalCount - 1,
                    lookupTotalCount: state.lookupTotalCount - 1
                }));
            })
        );
    }

    @Action(ClearService)
    clearService(ctx: StateContext<ServiceStateModel>) {
        ctx.setState(patch({ service: null }));
    }
    //#endregion
}
