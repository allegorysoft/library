import { FilteredPagedAndSortedResultRequestDto } from "@proxy";
import { UnitPriceCreateDto, UnitPriceUpdateDto } from "@proxy/unit-prices";

export class GetUnitPrices {
    static readonly type = '[Unit Price] Get Unit Prices';
    constructor(public payload: FilteredPagedAndSortedResultRequestDto) { }
}

export class GetUnitPrice {
    static readonly type = '[Unit Price] Get Unit Price';
    constructor(public payload: number) { }
}

export class CreateUnitPrice {
    static readonly type = '[Unit Price] Create Unit Price';
    constructor(public payload: UnitPriceCreateDto) { }
}

export class UpdateUnitPrice {
    static readonly type = '[Unit Price] Update Unit Price';
    constructor(public id: number, public payload: UnitPriceUpdateDto) { }
}

export class DeleteUnitPrice {
    static readonly type = '[Unit Price] Delete Unit Price';
    constructor(public payload: number) { }
}

export class ClearUnitPrice {
    static readonly type = '[Clear] Clear Unit Price';
}

export class PatchBusyValue {
    static readonly type = '[Unit Price] Patch Busy Value';
    constructor(public payload: boolean) { }
}
