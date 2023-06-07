import { OnInit, Component, ChangeDetectorRef, OnDestroy, Inject } from '@angular/core';
import { isNullOrUndefined } from '@abp/ng.core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Select, Store } from '@ngxs/store';
import {
  CalculableProductAggregateRootDto,
  CalculableProductDto,
  DiscountDto,
  ProductCalculatorService
} from '@proxy/calculations/product';
import { ClientDto } from '@proxy/clients';
import {
  OrderLineUpdateDto,
  OrderStatu,
  orderStatuOptions,
  OrderType,
  OrderUpdateDto,
  OrderWithDetailsDto
} from '@proxy/orders';
import { Observable, of, Subject } from 'rxjs';
import { catchError, filter, finalize, takeUntil } from 'rxjs/operators';
import { isEmptyOrSpaces } from 'src/app/shared/utils';
import { PatchBusyValue, UpdateOrder } from '../../actions';

import * as orderConsts from '../../consts';
import { OrderState } from '../../states';
import * as moment from 'moment';

import { OBSERVE, Observed, ObserveFn, OBSERVE_PROVIDER } from 'ng-observe';

import { GetCurrencyDailyExchanges } from 'src/app/modules/currencies/actions';
import { CurrencyDailyExchangeDto } from '@proxy/currencies';

const { required, maxLength } = Validators;

@Component({
  selector: 'app-order-edit',
  templateUrl: './order-edit.component.html',
  styleUrls: ['./order-edit.component.scss'],
  providers: [OBSERVE_PROVIDER]
})
export class OrderEditComponent implements OnInit, OnDestroy {
  //#region Fields
  private readonly _defaultCurrencyCode = 'USD';
  private destroy$: Subject<void> = new Subject<void>();
  private _currencies: Observed<CurrencyDailyExchangeDto[]>;
  private _orderDate: Date;
  private _currencyRate?: number;

  @Select(OrderState.getOrder) private _order: Observable<OrderWithDetailsDto>;
  @Select(state => state.order!.busy) busy$: Observable<boolean>;

  orderForm!: FormGroup;
  orderType: OrderType;
  orderStatuOptions = orderStatuOptions;
  clientDialogVisible: boolean = false;
  minFractionDigits: number = 2;
  maxFractionDigits: number = 5;
  discountDialogVisible: boolean = false;
  calculableProductAggregateRootDto!: CalculableProductAggregateRootDto;
  //#endregion

  //#region Utilities
  private buildForm(data: OrderWithDetailsDto): void {
    this.orderType = data!.type;
    this.orderForm = this.fb.group({
      id: [data!.id],
      type: [data!.type],//OrderLine da kdv vs. gibi alanları set etmek için bu property kontrol ediliyor
      number: [data!.number, [required, maxLength(orderConsts.maxNumberLength)]],
      date: [new Date(data!.date), required],
      statu: [data!.statu, required],
      clientCode: [data!.clientCode],
      totalDiscount: [data!.totalDiscount],
      totalVatBase: [data!.totalVatBase],
      totalVatAmount: [data!.totalVatAmount],
      totalGross: [data!.totalGross],
      discounts: this.fb.array(
        data!.discounts?.map(
          discount => this.fb.group({ ...discount, id: [null] })
        ) || []
      ),
      hasCurrency: [isNullOrUndefined(data!.currencyCode) ? false : true],
      currencyCode: [data!.currencyCode || null],
      currencyRate: [data!.currencyRate || null],
      currencyTotalVatBase: [data!.currencyTotalVatBase || null],
      currencyTotalVatAmount: [data!.currencyTotalVatAmount || null],
      currencyTotalDiscount: [data!.currencyTotalDiscount || null],
      currencyTotalGross: [data!.currencyTotalGross || null],
      lines: this.fb.array(
        data!.lines?.map(
          orderLine => this.fb.group({
            ...orderLine,
            price: [{ value: orderLine.price, disabled: orderLine.currencyPrice > 0 }],
            total: [{ value: orderLine.total, disabled: orderLine.currencyTotal > 0 }],
            currencyRate: [
              {
                value: orderLine.currencyRate,
                disabled: isNullOrUndefined(orderLine.currencyRate)
              }
            ],
            currencyPrice: [
              {
                value: orderLine.currencyPrice,
                disabled: isNullOrUndefined(orderLine.currencyPrice)
              }
            ],
            currencyTotal: [
              {
                value: orderLine.currencyTotal,
                disabled: isNullOrUndefined(orderLine.currencyTotal)
              }
            ],
            reserveDate: [
              !isNullOrUndefined(orderLine.reserveDate) ? new Date(orderLine.reserveDate) : null
            ],
            reserveQuantity: [
              {
                value: orderLine.reserveQuantity,
                disabled: isNullOrUndefined(orderLine.reserveDate)
              }
            ],
            units: [],
            discounts: this.fb.array(
              orderLine!.discounts?.map(
                discount => this.fb.group({ ...discount, id: [null] })
              ) || []
            )
          })
        ) || []
      )
    });

    if (data.currencyRate > 0)
      this.getCurrencyDailyExchanges();
  }

  private getCurrencyDailyExchanges(sub?: () => void): void {
    const date = <Date>this.orderForm.get('date').value!;
    this.store
      .dispatch(new GetCurrencyDailyExchanges(moment(date).format('YYYY-MM-DD')))
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.orderForm.get('hasCurrency')?.patchValue(true);

        const currencyCode = <string>this.orderForm.value!.currencyCode;
        if (isNullOrUndefined(currencyCode) && isEmptyOrSpaces(currencyCode))
          this.orderForm.get('currencyCode').patchValue(this._defaultCurrencyCode);

        if (!isNullOrUndefined(sub)) sub();
      });
  }

  private get orderLines(): FormArray {
    return this.orderForm!.get('lines') as FormArray;
  }

  private get input(): CalculableProductAggregateRootDto {
    const orderForm = this.orderForm.getRawValue()!;
    const lines = this.orderLines.getRawValue()!;
    const discounts = <DiscountDto[]>orderForm.discounts;

    const input = <CalculableProductAggregateRootDto>{
      currencyCode: <string>orderForm.currencyCode || null,
      currencyRate: +orderForm.currencyRate || null,
      lines: lines,
      discounts: discounts
    };
    if (isNullOrUndefined(input.currencyRate)) input.currencyCode = null;
    return input;
  }

  get currencies(): CurrencyDailyExchangeDto[] {
    return this._currencies.value;
  }
  //#endregion

  //#region Ctor
  constructor(
    private router: Router,
    private fb: FormBuilder,
    private store: Store,
    private cdr: ChangeDetectorRef,
    private readonly calculator: ProductCalculatorService,
    @Inject(OBSERVE) private observe: ObserveFn
  ) {
    this._currencies = this.observe(this.store.select(state => state!.currency!.dailyExchanges));
  }
  //#endregion

  //#region Methods
  ngOnInit(): void {
    this._order
      .pipe(takeUntil(this.destroy$))
      .subscribe(order => this.buildForm(order));
  }

  clientAfterSelect(client: ClientDto): void {
    if (client)
      this.orderForm.get('clientCode')?.patchValue(client.code)

    this.clientDialogVisible = false;
  }

  //#region Date & currency
  dateOnFocus(): void {
    this._orderDate = <Date>this.orderForm.value.date;
  }

  dateOnBlur(): void {
    const date = <Date>this.orderForm.get('date').value || null;
    if (isNullOrUndefined(date)) {
      this.orderForm.get('hasCurrency')?.patchValue(false);
      this.orderForm.get('currencyCode')?.patchValue(null);
      this.orderForm.get('currencyRate')?.patchValue(null);
      return;
    }

    const hasCurrency = <boolean>this.orderForm.get('hasCurrency').value || false;
    const isEqaul =
      moment(this._orderDate).format('YYYY-MM-DD') ===
      moment(date).format('YYYY-MM-DD');

    if (hasCurrency && !isEqaul) {
      this.changeCurrency();
      this._orderDate = undefined;
    }
  }

  changeCurrency(): void {
    const date = <Date>this.orderForm.get('date').value || null;
    if (!isNullOrUndefined(date)) {
      this.getCurrencyDailyExchanges(
        () => this.currencyOnChange(<string>this.orderForm.value!.currencyCode)
      );
    }
  }

  currencyOnChange(selectedCurrencyCode: string): void {
    const selectedCurrency = this.currencies.find(f => f.currencyCode === selectedCurrencyCode);
    const isPurchase = this.orderType === OrderType.Purchase;

    this.orderForm.get('currencyRate')?.patchValue(
      isPurchase ? selectedCurrency.rate1 : selectedCurrency.rate2
    );
    this.calculate();
  }

  currencyRateOnFocus(): void {
    this._currencyRate = +this.orderForm.get('currencyRate').value || null;
  }

  currencyRateOnBlur(): void {
    const currencyRate = +this.orderForm.get('currencyRate').value || null;

    if (isNullOrUndefined(currencyRate) || currencyRate <= 0) {
      this.orderForm.get('currencyRate').patchValue(null);
      this.orderForm.get('currencyCode').patchValue(null);
      this.orderForm.get('hasCurrency').patchValue(false);
    }

    if (this._currencyRate !== currencyRate)
      this.calculate();
  }

  private calculate(catchErr?: () => void): void {
    this.store.dispatch(new PatchBusyValue(true));
    this.calculator.calculateAggregateRoot(this.input)
      .pipe(
        takeUntil(this.destroy$),
        catchError(err => {
          if (!isNullOrUndefined(catchErr)) catchErr();
          return of<CalculableProductAggregateRootDto>(null);
        }),
        filter(f => !isNullOrUndefined(f)),
        finalize(() => { this.store.dispatch(new PatchBusyValue(false)); })
      ).subscribe(response => {
        this.orderForm.patchValue({ ...response });
      });
  }
  //#endregion

  //#region Discount
  editDiscounts(): void {
    const orderForm = this.orderForm.getRawValue()!;
    const lines: CalculableProductDto[] = [];

    orderForm.lines.filter(f => !isNullOrUndefined(f.productCode || null))
      .map(orderLine => {
        lines.push({
          id: +orderLine.id || null,
          quantity: +orderLine.quantity,
          price: +orderLine.price,
          vatRate: +orderLine.vatRate,
          isVatIncluded: <boolean>orderLine.isVatIncluded,
          discounts: <DiscountDto[]>orderLine.discounts || null,
          deductionCode: <string>orderLine.deductionCode || null,
          deductionPart1: +orderLine.deductionPart1 || null,
          deductionPart2: +orderLine.deductionPart2 || null,
          total: +orderLine.total || null,
          discountTotal: +orderLine.discountTotal || 0,
          vatBase: +orderLine.vatBase || 0,
          vatAmount: +orderLine.vatAmount || 0,
          calculatedTotal: +orderLine.calculatedTotal || 0,
          currencyCode: <string>orderLine.currencyCode || null,
          currencyRate: +orderLine.currencyRate || null,
          currencyPrice: +orderLine.currencyPrice || null,
          currencyTotal: +orderLine.currencyTotal || null
        });
      });

    this.calculableProductAggregateRootDto = {
      currencyCode: <string>orderForm.currencyCode || null,
      currencyRate: +orderForm.currencyRate || null,
      totalDiscount: +orderForm.totalDiscount || 0,
      totalVatBase: +orderForm.totalVatBase || 0,
      totalVatAmount: +orderForm.totalVatAmount || 0,
      totalGross: +orderForm.totalGross || 0,
      discounts: <DiscountDto[]>(orderForm.discounts || []),
      lines: lines
    };

    this.discountDialogVisible = true;
  }

  handleDiscounts(ev: CalculableProductAggregateRootDto): void {
    if (ev!.totalVatBase >= 0) {
      this.orderForm.patchValue({
        ...ev
        //Ask here to NGTurkey or any angular channel
        // discounst: this.fb.array(
        //   ev.discounts?.map(
        //     discount => this.fb.group({ ...discount })
        //   )
        // )
      });

      this.orderForm.setControl('discounts', this.fb.array(
        ev!.discounts?.map(
          discount => this.fb.group({ ...discount })
        )
      ));
      this.calculableProductAggregateRootDto = undefined;
    }
    this.discountDialogVisible = false;
  }
  //#endregion

  navigateOrders(): void {
    this.router.navigate([`/orders/${this.orderType == 0 ? 'purchase' : 'sales'}`]);
  }

  save(): void {
    const index = this.orderLines.controls.findIndex(f =>
      isNullOrUndefined(f.get('id')?.value || null) &&
      isEmptyOrSpaces(f.get('productCode')?.value)
    );

    if (index > -1) this.orderLines.removeAt(index);

    this.cdr.detectChanges();

    const orderForm = this.orderForm.getRawValue()!;
    const lines = <OrderLineUpdateDto[]>orderForm.lines;

    lines.map(line =>
      line.reserveDate = !isNullOrUndefined(line.reserveDate)
        ? moment(line.reserveDate).format('YYYY-MM-DD')
        : null
    )

    const input = <OrderUpdateDto>{
      id: +orderForm.id,
      number: <string>orderForm.number,
      date: moment(orderForm.date).format('YYYY-MM-DDTHH:mm'),
      clientCode: <string>orderForm.clientCode,
      statu: <OrderStatu>orderForm.statu,
      lines: orderForm.lines,
      discounts: <DiscountDto[]>orderForm.discounts,
      currencyCode: <string>orderForm.currencyCode || null,
      currencyRate: <number>orderForm.currencyRate || null,
      extraProperties: orderForm.extraProperties
    };

    if (isNullOrUndefined(input.currencyRate)) input.currencyCode = null;

    this.store
      .dispatch(new UpdateOrder(input.id, input))
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.navigateOrders());
  }

  ngOnDestroy(): void {
    this.orderForm.reset();
    this.destroy$.next();
    this.destroy$.complete();
  }
  //#endregion
}
