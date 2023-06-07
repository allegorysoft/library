import { State, Store, Action, Selector, StateContext } from "@ngxs/store";
import { FilteredPagedAndSortedResultRequestDto } from "@proxy";
import { UNIT_PRICE_STATE_TOKEN } from "../tokens";

import * as unitPriceConsts from '../consts';
import { Injectable } from "@angular/core";
import { ToasterService } from "@abp/ng.theme.shared";
import { UnitPriceDto, UnitPriceService, UnitPriceWithDetailsDto } from "@proxy/unit-prices";
import { ClearUnitPrice, CreateUnitPrice, DeleteUnitPrice, GetUnitPrice, GetUnitPrices, UpdateUnitPrice } from "../actions";
import { patch } from "@ngxs/store/operators";
import { finalize, tap } from "rxjs/operators";

export class UnitPriceStateModel {
    public unitPrices: UnitPriceDto[];
    public requestDto: FilteredPagedAndSortedResultRequestDto;
    public loading: boolean;
    public loadingUnitPrice: boolean;
    public unitPrice: UnitPriceWithDetailsDto;
    public totalCount: number;
    public error: any;
    public busy: boolean;
}

@State<UnitPriceStateModel>({
    name: UNIT_PRICE_STATE_TOKEN,
    defaults: {
        unitPrices: [],
        requestDto: {
            maxResultCount: unitPriceConsts.maxResultCount,
            skipCount: 0,
            conditions: null,
            sorting: null
        },
        loading: false,
        loadingUnitPrice: false,
        unitPrice: null,
        totalCount: 0,
        error: null,
        busy: false
    } as UnitPriceStateModel
})
@Injectable()
export class UnitPriceState {
    //#region Selectors
    @Selector()
    static getUnitPrices(state: UnitPriceStateModel): UnitPriceDto[] {
        return state.unitPrices || [];
    }

    @Selector()
    static getUnitPrice(state: UnitPriceStateModel): UnitPriceWithDetailsDto {
        return state.unitPrice || null;
    }

    @Selector()
    static getRequestDto(state: UnitPriceStateModel): FilteredPagedAndSortedResultRequestDto {
        return state.requestDto || null;
    }

    @Selector()
    static getTotalCount(state: UnitPriceStateModel): number {
        return state.totalCount || 0;
    }

    @Selector()
    static getLoading(state: UnitPriceStateModel): boolean {
        return state.loading || false;
    }

    @Selector()
    static getLoadingUnitPrice(state: UnitPriceStateModel): boolean {
        return state.loadingUnitPrice || false;
    }

    @Selector()
    static getBusy(state: UnitPriceStateModel): boolean {
        return state.busy || false;
    }
    //#endregion

    //#region Ctor
    constructor(
        private readonly unitPriceService: UnitPriceService,
        private readonly toasterService: ToasterService,
        private store: Store
    ) { }
    //#endregion

    //#region Actions
    @Action(GetUnitPrices)
    loadUnitPrices(ctx: StateContext<UnitPriceStateModel>, { payload }: GetUnitPrices) {
        ctx.setState(patch({ loading: true }));
        return this.unitPriceService.list(payload).pipe(
            tap(response => {
                ctx.setState(patch({
                    unitPrices: response.items,
                    totalCount: response.totalCount,

                }));
            }),
            finalize(() => ctx.setState(patch({ loading: false, requestDto: payload })))
        );
    }

    @Action(GetUnitPrice)
    loadUnitPrice(ctx: StateContext<UnitPriceStateModel>, { payload }: GetUnitPrice) {
        ctx.setState(patch({ loadingUnitPrice: true }));
        return this.unitPriceService.get(payload).pipe(
            tap(response => ctx.setState(patch({ unitPrice: response }))),
            finalize(() => ctx.setState(patch({ loadingUnitPrice: false })))
        );
    }

    @Action(CreateUnitPrice)
    createUnitPrice(ctx: StateContext<UnitPriceStateModel>, { payload }: CreateUnitPrice) {
        ctx.setState(patch({ busy: true }));
        return this.unitPriceService.create(payload).pipe(
            tap(response => {
                const state = ctx.getState();
                this.store.dispatch(new GetUnitPrices({ ...state.requestDto }));
                this.toasterService.success(
                    '::CreatedSuccessfullyMessage', '::SuccessTitleMessage'
                );
            }),
            finalize(() => ctx.setState(patch({ busy: false })))
        );
    }

    @Action(UpdateUnitPrice)
    updateUnitPrice(ctx: StateContext<UnitPriceStateModel>, { id, payload }: UpdateUnitPrice) {
        ctx.setState(patch({ busy: true }));
        return this.unitPriceService.update(id, payload).pipe(
            tap(() => {
                const state = ctx.getState();
                this.store.dispatch(new GetUnitPrices({ ...state.requestDto }));
                this.toasterService.success(
                    '::UpdatedSuccessfullyMessage', '::SuccessTitleMessage'
                );
            }),
            finalize(() => ctx.setState(patch({ busy: false })))
        );
    }

    @Action(DeleteUnitPrice)
    deleteUnitPrice(ctx: StateContext<UnitPriceStateModel>, { payload }: DeleteUnitPrice) {
        ctx.setState(patch({ busy: true }));
        return this.unitPriceService.delete(payload).pipe(
            tap(() => {
                this.toasterService.success('::DeletedSuccessfullyMessage', '::SuccessTitleMessage')
                const state = ctx.getState();
                ctx.setState(patch({
                    unitPrices: state.unitPrices.filter(f => f.id !== payload),
                    totalCount: state.totalCount - 1
                }))
            }),
            finalize(() => ctx.setState(patch({ busy: false })))
        );
    }

    @Action(ClearUnitPrice)
    clearUnitPrice(ctx: StateContext<UnitPriceStateModel>) {
        ctx.setState(patch({ unitPrice: null }));
    }
    //#endregion
}
