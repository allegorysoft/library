import { FilteredPagedAndSortedResultRequestDto } from "@proxy";
import { GetServiceLookupListDto, ServiceCreateDto, ServiceUpdateDto } from "@proxy/services";

export class GetServices {
    static readonly type = '[Services] Get Services';
    constructor(public payload: FilteredPagedAndSortedResultRequestDto) { }
}

export class UpdateRequestDto {
    static readonly type = '[Services] Update Request Dto';
    constructor(public payload: FilteredPagedAndSortedResultRequestDto) { }
}

export class PatchWorkOnLookup {
    static readonly type = '[Services] Patch Work On Lookup';
    constructor(public payload: boolean) { }
}

export class GetLookupServices {
    static readonly type = '[Services] Get Lookup Services';
    constructor(public payload: GetServiceLookupListDto) { }
}

export class UpdateLookupRequestDto {
    static readonly type = '[Services] Update Lookup Request Dto';
    constructor(public payload: GetServiceLookupListDto) { }
}

export class GetService {
    static readonly type = '[Services] Get Service';
    constructor(public payload: number) { }
}

export class GetServiceByCode {
    static readonly type = '[Services] Get Service By Code';
    constructor(public payload: string) { }
}

export class CreateService {
    static readonly type = '[Services] Create Service';
    constructor(public payload: ServiceCreateDto) { }
}

export class UpdateService {
    static readonly type = '[Services] Update Service';
    constructor(public id: number, public payload: ServiceUpdateDto) { }
}

export class DeleteService {
    static readonly type = '[Services] Delete Service';
    constructor(public payload: number) { }
}

export class ClearService {
    static readonly type = '[Services] Clear Service';
}
