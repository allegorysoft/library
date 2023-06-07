import { Action, Selector, State, StateContext, Store } from "@ngxs/store";
import { FilteredPagedAndSortedResultRequestDto } from "@proxy";
import { OrderDto, OrderService, OrderWithDetailsDto } from "@proxy/orders";
import { ORDER_STATE_TOKEN } from "../tokens";

import * as orderConsts from '../consts';
import { Injectable } from "@angular/core";
import { ToasterService } from "@abp/ng.theme.shared";
import { CreateOrder, DeleteOrder, GetOrder, GetOrders, PatchBusyValue, UpdateOrder, UpdateRequestDto } from "../actions";
import { finalize, tap } from "rxjs/operators";
import { PagedResultDto } from "@abp/ng.core";
import { patch } from "@ngxs/store/operators";

export class OrderStateModel {
    public orders: OrderDto[];
    public requestDto: FilteredPagedAndSortedResultRequestDto;
    public order: OrderWithDetailsDto;
    public totalCount: number;
    public error: any;
    public busy: boolean;
}

@State<OrderStateModel>({
    name: ORDER_STATE_TOKEN,
    defaults: {
        orders: [],
        requestDto: {
            maxResultCount: orderConsts.maxResultCount,
            skipCount: 0,
            conditions: null,
            sorting: null
        },
        order: null,
        totalCount: 0,
        error: null,
        busy: false
    } as OrderStateModel
})
@Injectable()
export class OrderState {
    //#region Selectors from state
    @Selector()
    static getOrders(state: OrderStateModel): OrderDto[] {
        return state.orders || [];
    }

    @Selector()
    static getOrder(state: OrderStateModel): OrderWithDetailsDto {
        return state.order || null;
    }

    @Selector()
    static getRequestDto(state: OrderStateModel): FilteredPagedAndSortedResultRequestDto {
        return state.requestDto || null;
    }
    //#endregion

    //#region Ctor
    constructor(
        private readonly orderService: OrderService,
        private toasterService: ToasterService,
        private store: Store
    ) { }
    //#endregion

    //#region Actions
    @Action(GetOrders)
    loadOrders(ctx: StateContext<OrderStateModel>, { payload }: GetOrders) {
        return this.orderService.list(payload).pipe(
            tap((response: PagedResultDto<OrderDto>) => {
                ctx.setState(patch({
                    orders: response.items,
                    totalCount: response.totalCount
                }))
            })
        );
    }

    @Action(GetOrder)
    loadOrder(ctx: StateContext<OrderStateModel>, { payload }: GetOrder) {
        return this.orderService.get(payload).pipe(
            tap((response: OrderWithDetailsDto) => {
                ctx.setState(patch({ order: response }));
            })
        );
    }

    @Action(CreateOrder)
    createOrder(ctx: StateContext<OrderStateModel>, { payload }: CreateOrder) {
        ctx.setState(patch({ busy: true }));
        return this.orderService.create(payload).pipe(
            tap((response: OrderWithDetailsDto) => {
                this.toasterService.success(
                    '::CreatedSuccessfullyMessage', '::SuccessTitleMessage'
                );
            }),
            finalize(() => ctx.setState(patch({ busy: false })))
        );
    }

    @Action(UpdateOrder)
    updateOrder(ctx: StateContext<OrderStateModel>, { id, payload }: UpdateOrder) {
        ctx.setState(patch({ busy: true }));
        return this.orderService.update(id, payload).pipe(
            tap((response: OrderWithDetailsDto) => {
                this.toasterService.success(
                    '::UpdatedSuccessfullyMessage', '::SuccessTitleMessage'
                );
            }),
            finalize(() => ctx.setState(patch({ busy: false })))
        );
    }

    @Action(UpdateRequestDto)
    updateRequestDto(ctx: StateContext<OrderStateModel>, { payload }: UpdateRequestDto) {
        ctx.setState(patch({ requestDto: payload }));
    }

    @Action(DeleteOrder)
    deleteOrder(ctx: StateContext<OrderStateModel>, { payload }: DeleteOrder) {
        return this.orderService.delete(payload).pipe(
            tap(() => {
                this.toasterService.success('::DeletedSuccessfullyMessage', '::SuccessTitleMessage');
                const state = ctx.getState();
                ctx.patchState({
                    ...state,
                    orders: state.orders.filter(f => f.id !== payload),
                    totalCount: state.totalCount - 1
                });
            })
        );
    }

    @Action(PatchBusyValue)
    patchBusyValue(ctx: StateContext<OrderStateModel>, { payload }: PatchBusyValue) {
        ctx.setState(patch({ busy: payload }));
    }
    //#endregion
}
