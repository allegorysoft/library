import { FilteredPagedAndSortedResultRequestDto } from "@proxy";
import { CurrencyCreateUpdateDto, CurrencyDailyExchangeCreateUpdateDto } from "@proxy/currencies";

export class GetCurrencies {
    static readonly type = '[Currency] Get Currencies';
    constructor(public payload: FilteredPagedAndSortedResultRequestDto) { }
}

export class UpdateRequestDto {
    static readonly type = '[Currency] Update Request Dto';
    constructor(public payload: FilteredPagedAndSortedResultRequestDto) { }
}

export class GetCurrency {
    static readonly type = '[Currency] Get Currency';
    constructor(public payload: number) { }
}

export class CreateCurrency {
    static readonly type = '[Currency] Create Currency';
    constructor(public payload: CurrencyCreateUpdateDto) { }
}

export class UpdateCurrency {
    static readonly type = '[Currency] Update Currency';
    constructor(public id: number, public payload: CurrencyCreateUpdateDto) { }
}

export class DeleteCurrency {
    static readonly type = '[Currency] Delete Currency';
    constructor(public payload: number) { }
}

export class ClearCurrency {
    static readonly type = '[Currency] Clear Currency';
}

export class GetCurrencyDailyExchanges {
    static readonly type = '[Currency] Get Currency Daily Exchanges';
    constructor(public payload: string) { }
}

export class EditCurrencyDailyExchange {
    static readonly type = '[Currency] Edit Currency Daily Exchange';
    constructor(public payload: CurrencyDailyExchangeCreateUpdateDto) { }
}

export class RefreshDailyExchanges {
    static readonly type = '[Currency] Refresh Daily Exchanges';
}
