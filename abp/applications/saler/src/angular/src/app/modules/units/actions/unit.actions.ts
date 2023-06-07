import { FilteredPagedAndSortedResultRequestDto } from "@proxy";
import { UnitGroupCreateDto, UnitGroupDto, UnitGroupUpdateDto } from "@proxy/units";

export class GetUnitGroups {
    static readonly type = '[Unit Group] Get Unit Groups';
    constructor(public payload: FilteredPagedAndSortedResultRequestDto) { }
}

export class UpdateRequestDto {
    static readonly type = '[Unit Group] Update Request Dto';
    constructor(public payload: FilteredPagedAndSortedResultRequestDto) { }
}

export class GetUnitGroup {
    static readonly type = '[Unit Group] Get Unit Group';
    constructor(public payload: number) { }
}

export class GetUnitGroupByCode {
    static readonly type = '[Unit Group] Get Unit Group By Code';
    constructor(public payload: string) { }
}
export class CreateUnitGroup {
    static readonly type = '[Unit Group] Create Unit Group';
    constructor(public payload: UnitGroupCreateDto) { }
}

export class UpdateUnitGroup {
    static readonly type = '[Unit Group] Update Unit Group';
    constructor(public id: number, public payload: UnitGroupUpdateDto) { }
}

export class DeleteUnitGroup {
    static readonly type = '[Unit Group] Delete Unit Group';
    constructor(public payload: number) { }
}

export class SelectUnitGroup {
    static readonly type = '[Unit Group] Select Unit Group';
    constructor(public payload: UnitGroupDto) { }
}

export class ClearSelectedUnitGroup {
    static readonly type = '[Unit Group] Clear Selected Unit Group';
}

export class ClearUnitGroup {
    static readonly type = '[Unit Group] Clear Unit Group';
}

export class UnitOccurError {
    static readonly type = '[Unit] Handle Error';
    constructor(public payload: any) { }
}

export class GetGlobalUnits {
    static readonly type = '[Unit Group] Get Global Units';
}
