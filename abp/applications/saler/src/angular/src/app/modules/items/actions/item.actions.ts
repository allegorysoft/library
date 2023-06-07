import { FilteredPagedAndSortedResultRequestDto } from "@proxy";
import { GetItemLookupListDto, ItemCreateDto, ItemUpdateDto } from "@proxy/items";

export class GetItems {
    static readonly type = '[Items] Get Items';
    constructor(public payload: FilteredPagedAndSortedResultRequestDto) { }
}

export class UpdateRequestDto {
    static readonly type = '[Items] Update Request Dto';
    constructor(public payload: FilteredPagedAndSortedResultRequestDto) { }
}

export class PatchWorkOnLookup {
    static readonly type = '[Items] Patch Work On Lookup';
    constructor(public payload: boolean) { }
}

export class GetLookupItems {
    static readonly type = '[Items] Get Lookup Items';
    constructor(public payload: GetItemLookupListDto) { }
}

export class UpdateLookupRequestDto {
    static readonly type = '[Items] Update Lookup Request Dto';
    constructor(public payload: GetItemLookupListDto) { }
}

export class GetItem {
    static readonly type = '[Items] Get Item';
    constructor(public payload: number) { }
}

export class GetItemByCode {
    static readonly type = '[Items] Get Item By Code';
    constructor(public payload: string) { }
}

export class CreateItem {
    static readonly type = '[Items] Create Item';
    constructor(public payload: ItemCreateDto) { }
}

export class UpdateItem {
    static readonly type = '[Items] Update Item';
    constructor(public id: number, public payload: ItemUpdateDto) { }
}

export class DeleteItem {
    static readonly type = '[Items] Delete Item';
    constructor(public payload: number) { }
}

export class ClearItem {
    static readonly type = '[Items] Clear Item';
}
