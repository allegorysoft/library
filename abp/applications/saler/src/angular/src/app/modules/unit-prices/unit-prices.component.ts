import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UnitPriceType } from '@proxy/unit-prices';

@Component({
    selector: 'app-unit-prices',
    template: `<app-unit-price-list [type] ="unitPriceType"></app-unit-price-list>`
})
export class UnitPricesComponent implements OnInit {
    unitPriceType!: UnitPriceType;

    //#region Ctor
    constructor(private route: ActivatedRoute) { }
    //#endregion

    //#region Methods
    ngOnInit(): void {
        //Refactor here
        this.route.url.subscribe(url => {
            const type = url[0].path;
            switch (type) {
                case 'item': this.unitPriceType = UnitPriceType.Item; break;
                case 'service': this.unitPriceType = UnitPriceType.Service; break;
            }
        });
    }
    //#endregion
}
