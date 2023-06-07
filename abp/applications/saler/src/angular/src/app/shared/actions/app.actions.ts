export class AppOccurError {
    static readonly type = '[App] Handle Error';
    constructor(public payload: any) { }
}

export class ToggleSidebar {
    static readonly type = '[App] Toggle Sidebar';
    constructor(public payload: boolean) { }
}
