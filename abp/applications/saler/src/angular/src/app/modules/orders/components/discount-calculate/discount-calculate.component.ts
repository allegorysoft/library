import { OnInit, Component, EventEmitter, Input, Output, OnDestroy } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import {
  CalculableProductAggregateRootDto,
  CalculableProductAggregateRootInputDto,
  CalculableProductDto,
  DiscountDto,
  ProductCalculatorService
} from '@proxy/calculations/product';
import { Observable, of, Subject } from 'rxjs';
import { catchError, filter, finalize, takeUntil } from 'rxjs/operators';
import { isNullOrUndefined } from '@abp/ng.core';

@Component({
  selector: 'app-discount-calculate',
  templateUrl: './discount-calculate.component.html',
  styleUrls: ['./discount-calculate.component.scss']
})
export class DiscountCalculateComponent implements OnInit, OnDestroy {
  //#region Fields
  private destroy$: Subject<void> = new Subject<void>();
  private _rate: number;
  private _total: number;

  @Input() index: number = -1;
  @Input() productAggregateRootDto!: CalculableProductAggregateRootDto;
  @Output() productAggregateRootDtoChanged =
    new EventEmitter<CalculableProductAggregateRootDto>();

  busy: boolean = false;
  orderLineForm!: FormGroup;
  //#endregion

  //#region Utilities
  private buildForm(): void {
    this.orderLineForm = this.fb.group({
      ...this.selectedOrderLine,
      discounts: this.fb.array(
        this.selectedOrderLine!.discounts?.map(
          discount => this.fb.group({ ...discount })
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
    this.busy = true;
    this.calculateAggregateRoot(catchErr).subscribe(response => {
      this.productAggregateRootDto = { ...response };
      this.orderLineForm.patchValue({ ...response.lines[this.index] });
      this.addNewRow();

      if (!isNullOrUndefined(sub)) sub();
    });
  }

  private hasChange = (value: number, value2: number) => value.toFixed(5) !== value2.toFixed(5)

  //#region Getters
  private get input(): CalculableProductAggregateRootInputDto {
    const aggregateRootDto = this.productAggregateRootDto;
    const lines = aggregateRootDto.lines;

    lines[this.index] = <CalculableProductDto>this.orderLineForm.value;
    lines[this.index].discounts = lines[this.index].discounts.filter(
      f => +f.rate > 0 || f.total > 0
    );

    const input = <CalculableProductAggregateRootInputDto>{
      currencyCode: <string>aggregateRootDto.currencyCode || null,
      currencyRate: +aggregateRootDto.currencyRate || null,
      discounts: aggregateRootDto.discounts,
      lines: aggregateRootDto.lines
    };
    if (isNullOrUndefined(input.currencyRate)) input.currencyCode = null;

    return input;
  }

  get selectedOrderLine(): any {
    return this.productAggregateRootDto.lines[this.index];
  }

  get discounts(): FormArray {
    return this.orderLineForm!.get('discounts') as FormArray;
  }

  get newRowLength(): number {
    return this.discounts.controls.filter(
      f => +f.get('rate')?.value < 1 && +f.get('total')?.value < 1
    ).length
  }
  //#endregion
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
    if (this.newRowLength < 1)
      this.discounts.push(this.fb.group({ id: [null], rate: [0], total: [0] }));
  }

  rateOnFocus(discountLine: FormGroup): void {
    this._rate = +discountLine.get('rate').value;
  }

  rateOnBlur(discountLine: FormGroup, index: number): void {
    const rate = +discountLine.get('rate').value;
    const total = +discountLine.get('total').value;

    if (rate <= 0) discountLine.get('rate').patchValue(0);

    if (this.hasChange(this._rate, rate)) {
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
    this._total = +discountLine.get('total').value;
  }

  totalOnBlur(discountLine: FormGroup, index: number): void {
    const total = +discountLine.get('total').value;

    if (total <= 0) discountLine.get('total').patchValue(0);

    if (this.hasChange(this._total, total)) {
      if (total <= 0) {
        this.removeDiscountLine(index);
        return;
      }

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
    const aggregateRootDto = this.productAggregateRootDto;
    const lines = aggregateRootDto.lines;
    const line = lines[this.index];
    line.discounts = (<DiscountDto[]>this.discounts.value).filter(f =>
      !isNullOrUndefined(f.rate) &&
      !isNullOrUndefined(f.total) &&
      !isNaN(f.rate) &&
      !isNaN(f.total) &&
      +f.rate > 0 &&
      +f.total > 0
    );
    this.productAggregateRootDtoChanged.emit(this.productAggregateRootDto);
  }

  ngOnDestroy(): void {
    this.orderLineForm.reset();
    this.destroy$.next();
    this.destroy$.complete();
  }
  //#endregion
}
