import { Injectable } from "@angular/core";
import { finalize, tap } from "rxjs/operators";
import { Action, Selector, State, StateContext, Store } from "@ngxs/store";
import { patch, removeItem } from "@ngxs/store/operators";
import { PagedResultDto } from "@abp/ng.core";
import { ToasterService } from "@abp/ng.theme.shared";
import { FilteredPagedAndSortedResultRequestDto } from "@proxy";
import {
    GetItemLookupListDto,
    ItemDto,
    ItemLookupDto,
    ItemService,
    ItemWithDetailsDto
} from "@proxy/items";
import {
    ClearItem,
    CreateItem,
    DeleteItem,
    GetItem,
    GetItemByCode,
    GetItems,
    GetLookupItems,
    PatchWorkOnLookup,
    UpdateItem,
    UpdateLookupRequestDto,
    UpdateRequestDto
} from "../actions";
import { ITEM_STATE_TOKEN } from "../tokens";
import * as itemConsts from '../consts';

export class ItemStateModel {
    public items: ItemDto[];
    public totalCount: number;
    public loading: boolean;
    public requestDto: FilteredPagedAndSortedResultRequestDto;

    public workOnLookup: boolean;
    public lookupItems: ItemLookupDto[];
    public lookupTotalCount: number;
    public loadingLookup: boolean;
    public lookupRequestDto: GetItemLookupListDto;

    public item: ItemWithDetailsDto;
    public busy: boolean;
}

@State<ItemStateModel>({
    name: ITEM_STATE_TOKEN,
    defaults: <ItemStateModel>{
        items: [],
        totalCount: 0,
        loading: false,
        requestDto: {
            maxResultCount: itemConsts.maxResultCount,
            skipCount: 0,
            conditions: null,
            sorting: null
        },

        workOnLookup: false,
        lookupItems: [],
        lookupTotalCount: 0,
        loadingLookup: false,
        lookupRequestDto: {
            maxResultCount: itemConsts.maxResultCount,
            skipCount: 0,
            conditions: null,
            sorting: null,
            date: null,
            isSales: null,
            clientCode: null
        },

        item: null,
        busy: false
    }
})
@Injectable()
export class ItemState {
    //#region Selectors from state
    @Selector()
    static getItems(state: ItemStateModel): ItemDto[] {
        return state.items || [];
    }

    @Selector()
    static getTotalCount(state: ItemStateModel): number {
        return state.totalCount || 0;
    }

    @Selector()
    static getLoading(state: ItemStateModel): boolean {
        return state.loading || false;
    }

    @Selector()
    static getLookupItems(state: ItemStateModel): ItemLookupDto[] {
        return state.lookupItems || [];
    }

    @Selector()
    static getLookupTotalCount(state: ItemStateModel): number {
        return state.lookupTotalCount || 0;
    }

    @Selector()
    static getLoadingLookup(state: ItemStateModel): boolean {
        return state.loadingLookup || false;
    }

    @Selector()
    static getItem(state: ItemStateModel): ItemWithDetailsDto {
        return state.item || null;
    }
    //#endregion

    //#region Ctor
    constructor(
        private readonly itemService: ItemService,
        private toasterService: ToasterService,
        private store: Store
    ) { }
    //#endregion

    //#region Actions
    @Action(GetItems)
    loadItems(ctx: StateContext<ItemStateModel>, { payload }: GetItems) {
        ctx.setState(patch({ loading: true }));
        return this.itemService.list(payload).pipe(
            tap((response: PagedResultDto<ItemDto>) => {
                ctx.setState(
                    patch({
                        items: response.items,
                        totalCount: response.totalCount
                    })
                )
            }),
            finalize(() => ctx.setState(patch({ loading: false })))
        );
    }

    @Action(UpdateRequestDto)
    updateRequestDto(ctx: StateContext<ItemStateModel>, { payload }: UpdateRequestDto) {
        ctx.setState(patch({ requestDto: payload }));
    }

    @Action(PatchWorkOnLookup)
    patchWorkOnLookup(ctx: StateContext<ItemStateModel>, { payload }: PatchWorkOnLookup) {
        ctx.setState(patch({ workOnLookup: payload }));
    }

    @Action(GetLookupItems)
    loadLookupItems(ctx: StateContext<ItemStateModel>, { payload }: GetLookupItems) {
        ctx.setState(patch({ loadingLookup: true }))
        return this.itemService.listItemLookup(payload).pipe(
            tap(response => {
                ctx.setState(
                    patch({
                        lookupItems: response.items,
                        lookupTotalCount: response.totalCount
                    })
                )
            }),
            finalize(() => ctx.setState(patch({ loadingLookup: false })))
        );
    }

    @Action(UpdateLookupRequestDto)
    updateLookupRequestDto(ctx: StateContext<ItemStateModel>, { payload }: UpdateLookupRequestDto) {
        ctx.setState(patch({ lookupRequestDto: payload }));
    }

    @Action(GetItem)
    loadItem(ctx: StateContext<ItemStateModel>, { payload }: GetItem) {
        return this.itemService.get(payload).pipe(
            tap((response: ItemWithDetailsDto) => {
                ctx.setState(patch({ item: response }));
            })
        );
    }

    @Action(GetItemByCode)
    loadItemByCode(ctx: StateContext<ItemStateModel>, { payload }: GetItemByCode) {
        return this.itemService.getByCode(payload).pipe(
            tap((response: ItemWithDetailsDto) => {
                ctx.setState(patch({ item: response }));
            })
        );
    }

    @Action(CreateItem)
    createItem(ctx: StateContext<ItemStateModel>, { payload }: CreateItem) {
        ctx.setState(patch({ busy: true }))
        return this.itemService.create(payload).pipe(
            tap(() => {
                const state = ctx.getState();

                this.store.dispatch(
                    state.workOnLookup
                        ? new GetLookupItems({ ...state.lookupRequestDto })
                        : new GetItems({ ...state.requestDto })
                );

                this.toasterService.success(
                    '::CreatedSuccessfullyMessage', '::SuccessTitleMessage'
                );
            }),
            finalize(() => ctx.setState(patch({ busy: false })))
        );
    }

    @Action(UpdateItem)
    updateItem(ctx: StateContext<ItemStateModel>, { id, payload }: UpdateItem) {
        ctx.setState(patch({ busy: true }))
        return this.itemService.update(id, payload).pipe(
            tap(() => {
                const state = ctx.getState();
                
                this.store.dispatch(
                    state.workOnLookup
                        ? new GetLookupItems({ ...state.lookupRequestDto })
                        : new GetItems({ ...state.requestDto })
                );
                
                this.toasterService.success(
                    '::UpdatedSuccessfullyMessage', '::SuccessTitleMessage'
                );
            }),
            finalize(() => ctx.setState(patch({ busy: false })))
        );
    }

    @Action(DeleteItem)
    deleteItem(ctx: StateContext<ItemStateModel>, { payload }: DeleteItem) {
        return this.itemService.delete(payload).pipe(
            tap(() => {
                this.toasterService.success('::DeletedSuccessfullyMessage', '::SuccessTitleMessage')
                const state = ctx.getState();
                ctx.setState(
                    patch({
                        items: removeItem<ItemDto>(f => f.id === payload),
                        lookupItems: removeItem<ItemLookupDto>(f => f.id === payload),
                        totalCount: state.totalCount - 1,
                        lookupTotalCount: state.lookupTotalCount - 1
                    })
                )
            })
        );
    }

    @Action(ClearItem)
    clearItem(ctx: StateContext<ItemStateModel>) {
        ctx.setState(patch({ item: null }));
    }
    //#endregion
}
