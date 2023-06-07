import { Injectable } from '@angular/core';
import { Action, Selector, State, StateContext } from '@ngxs/store';
import { patch } from '@ngxs/store/operators';
import { DeductionDto, ProductCalculatorService } from '@proxy/calculations/product';
import { finalize, tap } from 'rxjs/operators';
import { GetDeductions } from '../actions';
import { PRODUCT_CALCULATION_STATE_TOKEN } from '../tokens';

export class ProductCalculationStateModel {
    deductions: DeductionDto[];
    retrievingData: boolean;
}

@State<ProductCalculationStateModel>({
    name: PRODUCT_CALCULATION_STATE_TOKEN,
    defaults: <ProductCalculationStateModel>{
        deductions: [],
        retrievingData: false
    }
})
@Injectable()
export class ProductCalculationState {
    //#region Selectors from state
    @Selector()
    static getDeductions(state: ProductCalculationStateModel): DeductionDto[] {
        return state.deductions || [];
    }

    @Selector()
    static getRetrievingData(state: ProductCalculationStateModel): boolean {
        return state.retrievingData || false;
    }
    //#endregion

    //#region Ctor
    constructor(private readonly productCalculatorService: ProductCalculatorService) { }
    //#endregion

    //#region Actions
    @Action(GetDeductions)
    loadDeductions(ctx: StateContext<ProductCalculationStateModel>) {
        ctx.setState(patch({ retrievingData: true }))
        return this.productCalculatorService.getDeductions().pipe(
            tap((response: DeductionDto[]) => {
                ctx.setState(patch({ deductions: response }))
            }),
            finalize(() => ctx.setState(patch({ retrievingData: false })))
        );
    }
    //#endregion
}
