import { isNullOrUndefined } from '@abp/ng.core';
import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import {
  CalculableProductAggregateRootDto,
  CalculableProductAggregateRootInputDto,
  CalculableProductInputDto,
  DiscountDto,
  ProductCalculatorService
} from '@proxy/calculations/product';
import { Observable, of, Subject } from 'rxjs';
import { catchError, filter, finalize, takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-discount-aggregate-calculate',
  templateUrl: './discount-aggregate-calculate.component.html',
  styleUrls: ['./discount-aggregate-calculate.component.scss']
})
export class DiscountAggregateCalculateComponent implements OnInit, OnDestroy {
  //#region Fields
  private destroy$: Subject<void> = new Subject<void>();
  private _rate: number;
  private _total: number;

  @Input() calculableProductAggregateRootDto!: CalculableProductAggregateRootDto;
  @Output() calculableProductAggregateRootDtoChanged:
    EventEmitter<CalculableProductAggregateRootDto> =
    new EventEmitter<CalculableProductAggregateRootDto>();

  busy: boolean = false;
  calculableProductAggregateForm!: FormGroup;
  //#endregion

  //#region Utilities
  private buildForm(): void {
    const aggregateRootDto = this.calculableProductAggregateRootDto;
    this.calculableProductAggregateForm = this.fb.group({
      currencyCode: [aggregateRootDto.currencyCode || null],
      currencyRate: [aggregateRootDto.currencyRate || null],
      currencyTotalDiscount: [aggregateRootDto.currencyTotalDiscount || null],
      currencyTotalVatBase: [aggregateRootDto.currencyTotalVatBase || null],
      currencyTotalVatAmount: [aggregateRootDto.currencyTotalVatAmount || null],
      currencyTotalGross: [aggregateRootDto.currencyTotalGross || null],
      totalDiscount: [aggregateRootDto.totalDiscount || 0],
      totalVatBase: [aggregateRootDto.totalVatBase || 0],
      totalVatAmount: [aggregateRootDto.totalVatAmount || 0],
      totalGross: [aggregateRootDto.totalGross || 0],
      discounts: this.fb.array(
        aggregateRootDto.discounts?.map(
          discount => this.fb.group({ ...discount })
        ) || []
      ),
      lines: this.fb.array(
        aggregateRootDto.lines?.map(
          line => this.fb.group({
            ...line,
            discounts: this.fb.array(
              line!.discounts?.map(
                discount => this.fb.group({ ...discount })
              ) || []
            )
          })
        ) || []
      )
    });
    this.addNewRow();
  }

  private calculateAggregateRoot(
    catchErr?: () => void
  ): Observable<CalculableProductAggregateRootDto> {
    return this.productCalculatorService.calculateAggregateRoot(this.input)
      .pipe(
        takeUntil(this.destroy$),
        catchError(err => {
          if (!isNullOrUndefined(catchErr)) catchErr();
          return of<CalculableProductAggregateRootDto>(null);
        }),
        filter(f => !isNullOrUndefined(f)),
        finalize(() => this.busy = false)
      );
  }

  private calculate(catchErr?: () => void, sub?: () => void): void {
    this.calculateAggregateRoot(catchErr)
      .subscribe(response => {
        this.calculableProductAggregateForm.patchValue({ ...response });
        this.addNewRow();

        if (!isNullOrUndefined(sub)) sub();
      });
  }

  private get input(): CalculableProductAggregateRootInputDto {
    const aggregateRootForm = this.calculableProductAggregateForm.value!;
    const lines: CalculableProductInputDto[] = [];
    const discounts = (<DiscountDto[]>this.discounts?.value)?.filter(
      f => +f.rate > 0 || +f.total > 0
    );

    (<CalculableProductInputDto[]>aggregateRootForm.lines)?.map(line => {
      lines.push({
        id: line.id || null,
        quantity: +line.quantity,
        price: +line.price,
        vatRate: +line.vatRate,
        isVatIncluded: <boolean>line.isVatIncluded,
        total: +line.total,
        discounts: line.discounts,
        deductionCode: <string>line.deductionCode,
        deductionPart1: +line.deductionPart1 || null,
        deductionPart2: +line.deductionPart2 || null,
        currencyCode: <string>line.currencyCode || null,
        currencyPrice: +line.currencyPrice || null,
        currencyRate: +line.currencyRate || null,
        currencyTotal: +line.currencyTotal || null
      });
    });

    const input: CalculableProductAggregateRootInputDto = {
      currencyCode: <string>aggregateRootForm.currencyCode || null,
      currencyRate: +aggregateRootForm.currencyRate || null,
      lines: lines,
      discounts: discounts
    };

    if (isNullOrUndefined(input.currencyRate)) input.currencyCode = null;
    return input
  }

  get discounts(): FormArray {
    return this.calculableProductAggregateForm.get('discounts') as FormArray;
  }

  get newRowLength(): number {
    return this.discounts.controls.filter(
      f => +f.get('rate')?.value < 1 && +f.get('total')?.value < 1
    ).length
  }
  //#endregion

  //#region Ctor
  constructor(
    private readonly fb: FormBuilder,
    private readonly productCalculatorService: ProductCalculatorService
  ) { }
  //#endregion

  //#region Methods
  ngOnInit(): void { this.buildForm(); }

  addNewRow(): void {
    if (this.newRowLength < 1) {
      this.discounts.push(
        this.fb.group({
          id: [null],
          rate: [0],
          total: [0]
        })
      );
    }
  }

  rateOnFocus(discountLine: FormGroup): void {
    this._rate = +discountLine.get('rate')?.value;
  }

  rateOnBlur(discountLine: FormGroup, index: number): void {
    const rate = +discountLine.get('rate').value;
    const total = +discountLine.get('total').value;

    if (rate <= 0) discountLine.get('rate').patchValue(0);

    if (this._rate.toFixed(5) !== rate.toFixed(5)) {
      if (rate <= 0) {
        this.removeDiscountLine(index);
        return;
      }

      discountLine.get('total').patchValue(0);
      this.calculate(
        () => {
          setTimeout(() => discountLine.get('rate').patchValue(this._rate), 0);
          discountLine.get('total').patchValue(total);
        },
        () => this._total = +discountLine.get('total').value
      );
    }
  }

  totalOnFocus(discountLine: FormGroup): void {
    this._total = +discountLine.get('total')?.value;
  }

  totalOnBlur(discountLine: FormGroup, index: number): void {
    const total = +discountLine.get('total').value;

    if (total <= 0) discountLine.get('total').patchValue(0);

    if (this._total.toFixed(5) !== total.toFixed(5)) {
      if (total <= 0) {
        this.removeDiscountLine(index);
        return;
      }

      this.busy = true;
      this.calculate(
        () => setTimeout(() => discountLine.get('total').patchValue(this._total), 0),
        () => this._rate = +discountLine.get('rate').value
      );
    }
  }

  removeDiscountLine(index: number): void {
    this.discounts.removeAt(index);
    this.calculate();
  }

  save(): void {
    const form = this.calculableProductAggregateForm.value!;
    const discounts = (<DiscountDto[]>form.discounts)?.filter(f =>
      !isNullOrUndefined(f.rate) &&
      !isNullOrUndefined(f.total) &&
      !isNaN(f.rate) &&
      !isNaN(f.total) &&
      +f.rate > 0 &&
      +f.total > 0
    );
    const output: CalculableProductAggregateRootDto = {
      totalDiscount: +form.totalDiscount,
      totalVatBase: +form.totalVatBase,
      totalVatAmount: +form.totalVatAmount,
      totalGross: +form.totalGross,
      currencyCode: <string>form.currencyCode || null,
      currencyRate: +form.currencyRate || null,
      currencyTotalDiscount: +form.currencyTotalDiscount || null,
      currencyTotalVatBase: +form.currencyTotalVatBase || null,
      currencyTotalVatAmount: +form.currencyTotalVatAmount || null,
      currencyTotalGross: +form.currencyTotalGross || null,
      discounts: discounts,
      lines: form.lines
    }

    this.calculableProductAggregateRootDtoChanged.emit(output);
  }

  ngOnDestroy(): void {
    this.calculableProductAggregateForm.reset();
    this.destroy$.next();
    this.destroy$.complete();
  }
  //#endregion
}
