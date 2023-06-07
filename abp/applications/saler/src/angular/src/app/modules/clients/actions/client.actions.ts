import { FilteredPagedAndSortedResultRequestDto } from "@proxy";
import { ClientCreateDto, ClientUpdateDto } from "@proxy/clients";

export class GetClients {
    static readonly type = '[Clients] Get Clients';
    constructor(public payload: FilteredPagedAndSortedResultRequestDto) { }
}

export class UpdateRequestDto {
    static readonly type = '[Clients] Update Request Dto';
    constructor(public payload: FilteredPagedAndSortedResultRequestDto) { }
}

export class GetClient {
    static readonly type = '[Clients] Get Client';
    constructor(public payload: number) { }
}

export class CreateClient {
    static readonly type = '[Clients] Create Client';
    constructor(public payload: ClientCreateDto) { }
}

export class UpdateClient {
    static readonly type = '[Clients] Update Client';
    constructor(public id: number, public payload: ClientUpdateDto) { }
}

export class DeleteClient {
    static readonly type = '[Clients] Delete Client';
    constructor(public payload: number) { }
}

export class UpdateSavingValue {
    static readonly type = '[Clients] Update Saving Value';
    constructor(public payload: boolean) { }
}

export class ClearClient {
    static readonly type = '[Clients] Clear Client';
}

export class UnitOccurError {
    static readonly type = '[Clients] Handle Error';
    constructor(public payload: any) { }
}
