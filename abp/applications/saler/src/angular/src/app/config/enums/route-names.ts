export const enum eGeneralRouteNames {
    General = '::Menu:General',
    Currency = '::Menu:Currency',
    Currencies = '::Menu:Currencies',
    DailyExchanges = '::Menu:DailyExchanges'
}

export const enum eProductManagementRouteNames {
    ProductManagement = '::Menu:ProductManagement',
    UnitGroup = '::Menu:UnitGroup',
    Items = '::Menu:Items',
    Services = '::Menu:Services',
    UnitPrices = '::Menu:UnitPrices',
    Item = '::Menu:Item', //Depends UnitPrices
    Service = '::Menu:Service'//Depends UnitPrices
}

export const enum eClientManagementRouteNames {
    ClientManagement = '::Menu:ClientManagement',
    Clients = '::Menu:Clients'
}

export const enum eTransactionsRouteNames {
    Transactions = '::Menu:Transactions'
}

export const enum eOrderManagementRouteNames {
    OrderManagement = '::Menu:OrderManagement',
    Orders = '::Menu:Orders',
    Purchasing = '::Menu:Orders:Purchasing',
    Sales = '::Menu:Orders:Sales'
}
