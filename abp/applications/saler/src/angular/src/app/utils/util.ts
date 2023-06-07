import { Condition } from '@allegorysoft/filter';

export function toPascalCase(string) {
    return `${string}`
        .replace(new RegExp(/[-_]+/, 'g'), ' ')
        .replace(new RegExp(/[^\w\s]/, 'g'), '')
        .replace(
            new RegExp(/\s+(.)(\w*)/, 'g'),
            ($1, $2, $3) => `${$2.toUpperCase() + $3.toLowerCase()}`
        )
        .replace(new RegExp(/\w/), s => s.toUpperCase());
}


export function getOrder(sortOrder: number): string {
    return sortOrder === -1 ? 'desc' : 'asc'
}

export function getCondition(
    column: string,
    matchMode: string,
    value: any
): Condition {
    //#region refactor here
    let not = false;
    switch (matchMode) {
        case 'notEquals': matchMode = 'DoesntEquals'; break;
        case 'lt': matchMode = 'IsLessThan'; break;
        case 'lte': matchMode = 'IsLessThanOrEqualto'; break;
        case 'gt': matchMode = 'IsGreaterThan'; break;
        case 'gte': matchMode = 'IsGreaterThanOrEqualto'; break;
        case 'notContains': matchMode = 'Contains'; not = true; break;
    }
    //#endregion

    const condition = <Condition>{
        column: toPascalCase(column),
        operator: <any>toPascalCase(matchMode),
        value: value,
        not: not
    };
    return condition;
}
