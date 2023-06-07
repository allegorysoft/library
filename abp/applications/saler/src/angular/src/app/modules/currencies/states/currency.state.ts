import { Injectable } from "@angular/core";
import { Action, Selector, State, StateContext, Store } from "@ngxs/store";
import { FilteredPagedAndSortedResultRequestDto } from "@proxy";
import { CurrencyDailyExchangeDto, CurrencyDto, CurrencyService } from "@proxy/currencies";
import { CURRENCY_STATE_TOKEN } from "../tokens";

import * as currencyConsts from '../consts';
import { ToasterService } from "@abp/ng.theme.shared";
import { GetCurrencies, UpdateRequestDto, GetCurrency, DeleteCurrency, ClearCurrency, CreateCurrency, UpdateCurrency, GetCurrencyDailyExchanges, RefreshDailyExchanges, EditCurrencyDailyExchange } from "../actions";
import { tap, finalize } from "rxjs/operators";
import { patch, updateItem } from "@ngxs/store/operators";

export class CurrencyStateModel {
    public currencies: CurrencyDto[];
    public requestDto: FilteredPagedAndSortedResultRequestDto;
    public currency: CurrencyDto;
    public busy: boolean;
    public totalCount: number;
    public refresingDailyExchanges: boolean;
    public loadingExchanges: boolean;
    public dailyExchanges: CurrencyDailyExchangeDto[];
    public error: any;
}


@State<CurrencyStateModel>({
    name: CURRENCY_STATE_TOKEN,
    defaults: <CurrencyStateModel>{
        currencies: [],
        requestDto: {
            maxResultCount: currencyConsts.maxResultCount,
            skipCount: 0,
            conditions: null,
            sorting: null
        },
        currency: null,
        busy: false,
        totalCount: 0,
        refresingDailyExchanges: false,
        loadingExchanges: false,
        dailyExchanges: [],
        error: null
    }
})
@Injectable()
export class CurrencyState {
    //#region Selectors from state
    @Selector()
    static getCurrencies(state: CurrencyStateModel): CurrencyDto[] {
        return state.currencies || [];
    }

    @Selector()
    static getTotalCount(state: CurrencyStateModel): number {
        return state.totalCount || 0;
    }

    @Selector()
    static getCurrency(state: CurrencyStateModel): CurrencyDto {
        return state.currency || null;
    }

    @Selector()
    static getRequestDto(state: CurrencyStateModel): FilteredPagedAndSortedResultRequestDto {
        return state.requestDto;
    }

    @Selector()
    static getBusy(state: CurrencyStateModel): boolean {
        return state.busy || false;
    }

    @Selector()
    static getRefresingDailyExchanges(state: CurrencyStateModel): boolean {
        return state.refresingDailyExchanges || false;
    }

    @Selector()
    static getLoadingExchanges(state: CurrencyStateModel): boolean {
        return state.loadingExchanges || false;
    }
    @Selector()
    static getDailyExchanges(state: CurrencyStateModel): CurrencyDailyExchangeDto[] {
        return state.dailyExchanges || [];
    }
    //#endregion

    //#region Ctor
    constructor(
        private readonly currencyService: CurrencyService,
        private readonly toasterService: ToasterService,
        private readonly store: Store
    ) { }
    //#endregion

    //#region Actions
    @Action(GetCurrencies)
    loadCurrencies(ctx: StateContext<CurrencyStateModel>, { payload }: GetCurrencies) {
        return this.currencyService.list(payload).pipe(
            tap(response => {
                ctx.setState(
                    patch({
                        currencies: response.items || [],
                        totalCount: response.totalCount
                    })
                );
            })
        );
    }

    @Action(UpdateRequestDto)
    updateRequestDto(ctx: StateContext<CurrencyStateModel>, { payload }: UpdateRequestDto) {
        ctx.setState(patch({ requestDto: payload }));
    }

    @Action(GetCurrency)
    loadCurrency(ctx: StateContext<CurrencyStateModel>, { payload }: GetCurrency) {
        return this.currencyService.get(payload).pipe(
            tap((response: CurrencyDto) => ctx.setState(patch({ currency: response })))
        );
    }

    @Action(CreateCurrency)
    createCurrency(ctx: StateContext<CurrencyStateModel>, { payload }: CreateCurrency) {
        ctx.setState(patch({ busy: true }));
        return this.currencyService.create(payload).pipe(
            tap(response => {
                const state = ctx.getState();
                this.store.dispatch(new GetCurrencies({ ...state.requestDto }));
                this.toasterService.success('::CreatedSuccessfullyMessage', '::SuccessTitleMessage');
            }),
            finalize(() => ctx.setState(patch({ busy: false })))
        );
    }

    @Action(UpdateCurrency)
    updateUnitGroup(ctx: StateContext<CurrencyStateModel>, { id, payload }: UpdateCurrency) {
        ctx.setState(patch({ busy: true }));
        return this.currencyService.update(id, payload).pipe(
            tap(response => {
                ctx.setState(patch({
                    currencies: updateItem<CurrencyDto>(
                        f => f.id === response.id, { ...response }
                    )
                }))
                this.toasterService.success(
                    '::UpdatedSuccessfullyMessage', '::SuccessTitleMessage'
                );
            }),
            finalize(() => ctx.setState(patch({ busy: false })))
        );
    }

    @Action(DeleteCurrency)
    deleteCurrency(ctx: StateContext<CurrencyStateModel>, { payload }: DeleteCurrency) {
        return this.currencyService.delete(payload).pipe(
            tap(() => {
                this.toasterService.success('::DeletedSuccessfullyMessage', '::SuccessTitleMessage')
                const state = ctx.getState();
                ctx.setState(patch({
                    currencies: state.currencies.filter(f => f.id !== payload),
                    totalCount: state.totalCount - 1
                }))
            })
        );
    }

    @Action(ClearCurrency)
    clearUnitGroup(ctx: StateContext<CurrencyStateModel>) {
        ctx.setState(patch({ currency: null }))
    }

    @Action(GetCurrencyDailyExchanges)
    loadDailyExchanges(ctx: StateContext<CurrencyStateModel>, { payload }: GetCurrencyDailyExchanges) {
        ctx.setState(patch({ loadingExchanges: true }))
        return this.currencyService.getCurrencyDailyExchangeList(payload).pipe(
            tap(response =>
                ctx.setState(
                    patch({ dailyExchanges: response || [] })
                )
            ),
            finalize(() => ctx.setState(patch({ loadingExchanges: false })))
        );
    }

    @Action(EditCurrencyDailyExchange)
    editCurrencyDailyExchange(ctx: StateContext<CurrencyStateModel>, { payload }: EditCurrencyDailyExchange) {
        return this.currencyService.editCurrencyDailyExchange(payload).pipe(
            tap(() => {
                this.toasterService.success('::UpdatedSuccessfullyMessage', '::SuccessTitleMessage');
            })
        );
    }

    @Action(RefreshDailyExchanges)
    refreshDailyExchanges(ctx: StateContext<CurrencyStateModel>) {
        ctx.setState(patch({ refresingDailyExchanges: true }))
        return this.currencyService.refreshDailyExchanges().pipe(
            tap(() => {
                this.toasterService.info(
                    '::DailyExchangesRefreshedMessage', '::InformationTitleMessage'
                );
            }),
            finalize(() => ctx.setState(patch({ refresingDailyExchanges: false })))
        );
    }
    //#endregion
}
