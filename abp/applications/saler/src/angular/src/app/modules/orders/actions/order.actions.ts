import { FilteredPagedAndSortedResultRequestDto } from "@proxy";
import { OrderCreateDto, OrderUpdateDto } from "@proxy/orders";

export class GetOrders {
    static readonly type = '[Orders] Get Orders';
    constructor(public payload: FilteredPagedAndSortedResultRequestDto) { }
}

export class GetOrder {
    static readonly type = '[Orders] Get Order';
    constructor(public payload: number) { }
}

export class CreateOrder {
    static readonly type = '[Orders] Create Order';
    constructor(public payload: OrderCreateDto) { }
}

export class UpdateOrder {
    static readonly type = '[Orders] Update Order';
    constructor(public id: number, public payload: OrderUpdateDto) { }
}

export class UpdateRequestDto {
    static readonly type = '[Orders] Update Request Dto';
    constructor(public payload: FilteredPagedAndSortedResultRequestDto) { }
}

export class DeleteOrder {
    static readonly type = '[Orders] Delete Order';
    constructor(public payload: number) { }
}

export class PatchBusyValue {
    static readonly type = '[Orders] Patch Busy Value';
    constructor(public payload: boolean) { }
}