export function isEmptyOrSpaces(str): boolean {
    return str === null || str.match(/^ *$/) !== null;
}
