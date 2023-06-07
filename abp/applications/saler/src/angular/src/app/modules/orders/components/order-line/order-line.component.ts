import {
  OnInit,
  Component,
  Inject,
  Input,
  ChangeDetectorRef,
  OnDestroy
} from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ABP, isNullOrUndefined } from '@abp/ng.core';
import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { ItemLookupDto, ItemWithDetailsDto } from '@proxy/items';
import { OrderLineType, orderLineTypeOptions, OrderType } from '@proxy/orders';
import { ServiceLookupDto, ServiceWithDetailsDto } from '@proxy/services';
import { UnitDto, UnitGroupWithDetailsDto } from '@proxy/units';
import {
  CalculableProductAggregateRootDto,
  CalculableProductAggregateRootInputDto,
  CalculableProductInputDto,
  DeductionDto,
  DiscountDto,
  ProductCalculatorService
} from '@proxy/calculations/product';
import { CurrencyDailyExchangeDto } from '@proxy/currencies';
import { Select, Store } from '@ngxs/store';
import { Observable, of, Subject } from 'rxjs';
import { catchError, filter, finalize, takeUntil } from 'rxjs/operators';
import { isEmptyOrSpaces } from 'src/app/shared/utils';
import { ClearItem, GetItemByCode } from 'src/app/modules/items/actions';
import { ClearService, GetServiceByCode } from 'src/app/modules/services/actions';
import { ClearUnitGroup, GetUnitGroupByCode } from 'src/app/modules/units/actions';
import { GetCurrencyDailyExchanges } from 'src/app/modules/currencies/actions';
import { OBSERVE, Observed, ObserveFn } from 'ng-observe';
import * as moment from 'moment';
import { UnitPriceService, UnitPriceType } from '@proxy/unit-prices';

@Component({
  selector: 'app-order-line',
  templateUrl: './order-line.component.html',
  styleUrls: ['./order-line.component.scss']
})
export class OrderLineComponent implements OnInit, OnDestroy {
  //#region Fields
  private destroy$: Subject<void> = new Subject<void>();
  private _productCode: string;
  private _quantity: number;
  private _price: number;
  private _vatRate: number;
  private _total: number;
  private _discountTotal: number;
  private _deductionCode: string; ü
  private _deductionPart1?: number;
  private _deductionPart2?: number;
  private _currencyCode?: string;
  private _currencyRate?: number;
  private _currencyPrice?: number;
  private _currencyTotal?: number;
  private _reserveDate?: Date;
  private _reserveQuantity?: number;
  private _selectedRow: FormGroup;

  @Input() orderForm!: FormGroup;
  @Select(state => state.order!.busy) busy$: Observable<boolean>;

  orderLineTypeOptions: ABP.Option<typeof OrderLineType>[] = orderLineTypeOptions;
  dialogVisible: boolean = false;
  itemListVisible: boolean = false;
  serviceListVisible: boolean = false;
  gettingData: boolean = false;
  selectedIndex: number;
  discountDialogVisible: boolean = false;
  deductionListSelectDialogVisible: boolean = false;
  productAggregateRootDto!: CalculableProductAggregateRootDto;
  currencies: Observed<CurrencyDailyExchangeDto[]>;
  //#endregion

  //#region Utilities
  //#region Service requests
  private getItemByCode(code: string): Observable<any> {
    return this.store.dispatch(new GetItemByCode(code))
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => {
          this.store.dispatch(new ClearItem());
          this.gettingData = false;
        })
      );
  }

  private getServiceByCode(code: string): Observable<any> {
    return this.store.dispatch(new GetServiceByCode(code))
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => {
          this.store.dispatch(new ClearService());
          this.gettingData = false;
        })
      );
  }

  private getUnitGroupByCode(code: string): Observable<any> {
    return this.store.dispatch(new GetUnitGroupByCode(code))
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => {
          this.store.dispatch(new ClearUnitGroup());
          this.gettingData = false;
        })
      );
  }
  //#endregion

  private setMainUnit(orderLine: FormGroup): void {
    const unitGroupCode = <string>orderLine.get('unitGroupCode').value;
    this.getUnitGroupByCode(unitGroupCode).subscribe(state => {
      const unitGroup = <UnitGroupWithDetailsDto>state!.unit!.unitGroup;
      const unit = unitGroup.units.find(f => f.mainUnit === true);
      orderLine.get('unitCode').patchValue(unit.code);

      const quantity = +orderLine.get('quantity').value;
      if (quantity > 0 && quantity % 1 !== 0 && !unit.divisible)
        orderLine.get('quantity').patchValue(Math.round(quantity));

      const isVatIncluded = <boolean>orderLine.get('isVatIncluded').value;
      this.unitPriceService
        .getPrice(
          <string>orderLine.get('productCode').value,
          <UnitPriceType>orderLine.get("type").value,
          unit.code,
          this.orderDate,
          this.orderType === OrderType.Sales,
          isVatIncluded ? +orderLine.get('vatRate').value : null,
          <string>orderLine.get('currencyCode').value || null,
          this.clientCode
        )
        .pipe(takeUntil(this.destroy$))
        .subscribe(response => {
          const price = +orderLine.get('price').value;
          const total = +orderLine.get('total').value;

          if (response > 0)
            orderLine.get('total').patchValue(0);

          orderLine.get('price').patchValue(response);

          this.calculate(
            () => {
              orderLine.get('price').patchValue(price);
              orderLine.get('total').patchValue(total);
            }
          );
        });
    });
  }

  private setLineValues(orderLine: FormGroup, value: any): void {
    //#region set value
    const isPurchase = this.orderType == OrderType.Purchase;
    const purchaseVatRate = +value.purchaseVatRate;
    const salesVatRate = +value.salesVatRate;

    orderLine.get('productCode').patchValue(value.code);
    orderLine.get('productName').patchValue(value.name);
    orderLine.get('unitGroupCode').patchValue(value.unitGroupCode);

    //#region Deduction
    const deductionCode = <string>value.deductionCode || null;
    const purchasePart1 = +value.purchaseDeductionPart1 || null;
    const purchasePart2 = +value.purchaseDeductionPart2 || null;
    const salesPart1 = +value.salesDeductionPart1 || null;
    const salesPart2 = +value.salesDeductionPart2 || null;

    orderLine.get('deductionCode').patchValue(null);

    if (!isNullOrUndefined(deductionCode)) {
      const control = orderLine.get('deductionCode');

      if (isPurchase && (purchasePart1 > 0 && purchasePart2 > 0))
        control.patchValue(deductionCode);
      else if (!isPurchase && (salesPart1 > 0 && salesPart2 > 0))
        control.patchValue(deductionCode);
      else
        control.patchValue(null);
    }

    orderLine.get('deductionPart1').patchValue(
      isPurchase ? purchasePart1 : salesPart1
    );
    orderLine.get('deductionPart2').patchValue(
      isPurchase ? purchasePart2 : salesPart2
    );
    //#endregion

    orderLine.get('vatRate').patchValue(isPurchase ? purchaseVatRate : salesVatRate);
    //#endregion

    this.setMainUnit(orderLine);
    this.selectedIndex = -1;
  }

  private hasProductCode(orderLine: FormGroup): boolean {
    const productCode = <string>orderLine.get('productCode').value || null;
    return !isNullOrUndefined(productCode);
  }

  private validateDeduction = (
    code?: string,
    part1?: number,
    part2?: number
  ): boolean =>
    !isNullOrUndefined(code) &&
    !isNullOrUndefined(part1) &&
    !isNullOrUndefined(part2)

  private hasChange = (value: number, value2: number) => value.toFixed(5) !== value2.toFixed(5)

  private checkDivisible(orderLine: FormGroup): void {
    const calculatedQuantity = +orderLine.get('quantity').value;

    if (calculatedQuantity > 0 && calculatedQuantity % 1 !== 0) {
      const unitGroupCode = <string>orderLine.get('unitGroupCode').value;

      this.getUnitGroupByCode(unitGroupCode).subscribe(state => {
        const unitGroup = <UnitGroupWithDetailsDto>state!.unit!.unitGroup;
        const unit = unitGroup.units.find(
          f => f.code === <string>orderLine.get('unitCode').value
        );

        if (!unit.divisible) {
          orderLine.get('quantity').patchValue(Math.round(calculatedQuantity));
          setTimeout(() => this.calculate(), 0);
        }
      });
    }
  }

  private calculate(catchErr?: () => void, sub?: () => void): void {
    this.calculateAggregateRoot(catchErr).subscribe(response => {
      this.orderForm.patchValue({ ...response });
      if (!isNullOrUndefined(sub)) sub();
    });
  }

  private calculateAggregateRoot(catchErr?: () => void): Observable<any> {
    this.gettingData = true;
    return this.calculator.calculateAggregateRoot(this.input)
      .pipe(
        takeUntil(this.destroy$),
        catchError(err => {
          if (!isNullOrUndefined(catchErr)) catchErr();
          return of<CalculableProductAggregateRootDto>(null);
        }),
        filter(f => !isNullOrUndefined(f)),
        finalize(() => this.gettingData = false)
      );
  }

  private clearLineValues(orderLine: FormGroup): void {
    const lineTotal = +orderLine.get('total').value;

    orderLine.get('productCode').patchValue('');
    orderLine.get('productName').patchValue('');
    orderLine.get('unitGroupCode').patchValue('');
    orderLine.get('unitCode').patchValue('');
    orderLine.get('price').patchValue(0);
    orderLine.get('vatRate').patchValue(0);
    orderLine.get('vatBase').patchValue(0);
    orderLine.get('vatAmount').patchValue(0);
    orderLine.get('isVatIncluded').patchValue(false);
    orderLine.get('total').patchValue(0);
    orderLine.get('discountTotal').patchValue(0);
    orderLine.setControl('discounts', this.fb.array([]));
    orderLine.get('calculatedTotal').patchValue(0);
    orderLine.get('deductionCode').patchValue(null);
    orderLine.get('deductionPart1').patchValue(null);
    orderLine.get('deductionPart2').patchValue(null);
    orderLine.get('currencyCode').patchValue(null);
    orderLine.get('currencyRate').patchValue(null);
    orderLine.get('currencyPrice').patchValue(null);
    orderLine.get('currencyTotal').patchValue(null);

    if (+this.orderForm.get('totalGross').value > 0 && lineTotal > 0)
      this.calculate();
  }

  private clearDiscounts(orderLine: FormGroup): void {
    orderLine.get('discountTotal').patchValue(0);
    orderLine.setControl('discounts', this.fb.array([]));
  }

  private clearCurrencPriceAndTotal(orderLine: FormGroup): void {
    orderLine.get('currencyPrice').patchValue(null);
    orderLine.get('currencyTotal').patchValue(null);
  }

  private clearCurrency(orderLine: FormGroup): void {
    orderLine.get('currencyCode').patchValue(null);
    orderLine.get('currencyRate').patchValue(null);
    this.clearCurrencPriceAndTotal(orderLine);
  }

  //#region Getters
  private get orderType(): OrderType {
    return <OrderType>this.orderForm.get('type').value;
  }

  private get input(): CalculableProductAggregateRootInputDto {
    const orderForm = this.orderForm.getRawValue()!;
    const lines = <any[]>this.orderLines.getRawValue()!;

    //TODO: Refactor input dto in lines. Remove unnecessary properties
    const input = <CalculableProductAggregateRootInputDto>{
      currencyCode: <string>orderForm.currencyCode || null,
      currencyRate: +orderForm.currencyRate || null,
      discounts: <DiscountDto[]>this.orderDiscounts.value,
      lines: <CalculableProductInputDto[]>lines.filter(f =>
        !isNullOrUndefined(f.productCode || null)
      )
    };

    if (isNullOrUndefined(input.currencyRate)) input.currencyCode = null;
    return input;
  }

  get orderDiscounts(): FormArray {
    return this.orderForm.get('discounts') as FormArray;
  }

  get orderLines(): FormArray {
    return this.orderForm!.get('lines') as FormArray;
  }

  get newRowLength(): number {
    return this.orderLines.controls.filter(f =>
      isEmptyOrSpaces(f.get('productCode')?.value) && !f.get('id')?.value
    ).length;
  }

  get dialogTitle(): string {
    return this.itemListVisible ? '::SelectItem' : '::SelectService';
  }

  get orderDate(): string {
    const date = this.orderForm.get('date')?.value || null;
    return !isNullOrUndefined(date)
      ? moment(this.orderForm.get('date').value).format('YYYY-MM-DDTHH:mm')
      : null;
  }

  get clientCode(): string {
    return <string>this.orderForm.get('clientCode')?.value || null;
  }
  //#endregion
  //#endregion

  //#region Ctor
  constructor(
    @Inject(OBSERVE) private observe: ObserveFn,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef,
    private store: Store,
    private readonly confirmation: ConfirmationService,
    private readonly calculator: ProductCalculatorService,
    private readonly unitPriceService: UnitPriceService
  ) {
    this.currencies = this.observe(this.store.select(state => state!.currency!.dailyExchanges));
  }
  //#endregion

  //#region Methods
  ngOnInit(): void {
    this.addNewRow();
    this.orderForm.markAllAsTouched();
  }

  //#region OrderLine
  addNewRow(): void {
    if (this.newRowLength < 1) {
      this.orderLines.push(
        this.fb.group({
          type: [OrderLineType.Item],
          productCode: [''],
          productName: [''],
          unitGroupCode: [''],
          unitCode: [''],
          quantity: [0],
          price: [0],
          vatRate: [0],
          vatBase: [0],
          isVatIncluded: [false],
          vatAmount: [0],
          total: [0],
          discountTotal: [0],
          discounts: [[]],
          calculatedTotal: [0],
          deductionCode: [null],
          deductionPart1: [null],
          deductionPart2: [null],
          units: [[]],
          currencyCode: [null],
          currencyRate: [{ value: null, disabled: true }],
          currencyPrice: [{ value: null, disabled: true }],
          currencyTotal: [{ value: null, disabled: true }],
          reserveDate: [null],
          reserveQuantity: [null]
        })
      );
    }
  }

  removeOrderLine(index: number, orderLine: FormGroup): void {
    const productCode = <string>orderLine.get('productCode').value || null;
    if (!isNullOrUndefined(productCode)) {
      this.confirmation
        .warn('::OrderLineDeletionConfirmationMessage', '::AreYouSure', {
          messageLocalizationParams: [productCode]
        })
        .pipe(takeUntil(this.destroy$))
        .subscribe(response => {
          if (response == Confirmation.Status.confirm) {
            this.orderLines.removeAt(index);
            this.calculate();
          }
        });
    }
    else if (this.newRowLength > 1) this.orderLines.removeAt(index);
  }

  lineTypeOnChange = (orderLine: FormGroup): any => this.clearLineValues(orderLine);
  //#endregion

  //#region OrderLine product
  productCodeOnFocus(orderLine: FormGroup): void {
    this._productCode = <string>orderLine.get('productCode').value || null;
  }

  productCodeOnBlur(orderLine: FormGroup): void {
    const productCode = <string>orderLine.get("productCode").value || null;

    if (!isEmptyOrSpaces(productCode) && this._productCode !== productCode) {
      this.gettingData = true;
      switch (<OrderLineType>orderLine.get("type").value) {
        case OrderLineType.Item:
          const reqItem = this.getItemByCode(productCode);

          reqItem.subscribe(state => {
            const item = <ItemWithDetailsDto>state!.item!.item;
            this.setLineValues(orderLine, item);
          }, () => orderLine.get('productCode').patchValue(this._productCode));
          break;

        case OrderLineType.Service:
          const reqService = this.getServiceByCode(productCode);

          reqService.subscribe((state) => {
            const service = <ServiceWithDetailsDto>state!.service!.service;
            this.setLineValues(orderLine, service);
          }, () => orderLine.get('productCode').patchValue(this._productCode));
          break;
        default: this.gettingData = false; break;
      }
      this.addNewRow();
    }
    else if (isNullOrUndefined(productCode)) this.clearLineValues(orderLine);
  }

  productNameOnFocus(orderLine: FormGroup): void {
    this._productCode = orderLine.get('productCode').value || null;
  }

  showProductDialog(index: number, orderLine: FormGroup): void {
    const lineType = +orderLine.get('type').value;

    if (lineType > -1 && lineType < 2) {
      this.dialogVisible = true;
      switch (<OrderLineType>lineType) {
        case OrderLineType.Item: this.itemListVisible = true; break;
        case OrderLineType.Service: this.serviceListVisible = true; break;
      }
      this.selectedIndex = index;
    }
  }

  productDialogOnHide(): void {
    this.dialogVisible = this.itemListVisible = this.serviceListVisible = false;
  }

  itemAfterSelect(selectedItem: ItemLookupDto): void {
    if (this.selectedIndex > -1 && selectedItem) {
      this.gettingData = true;

      this.getItemByCode(selectedItem?.code)
        .subscribe(state => {
          const item = <ItemWithDetailsDto>state!.item!.item;
          const orderLine = <FormGroup>this.orderLines.controls[this.selectedIndex];
          this.setLineValues(orderLine, item);
          this.addNewRow();
        });
      this.productDialogOnHide();
    }
  }

  serviceAfterSelect(selectedService: ServiceLookupDto): void {
    if (this.selectedIndex > -1 && selectedService) {
      this.gettingData = true;

      this.getServiceByCode(selectedService?.code)
        .subscribe(state => {
          const service = <ServiceWithDetailsDto>state!.service!.service;
          const orderLine = <FormGroup>this.orderLines.controls[this.selectedIndex];
          this.setLineValues(orderLine, service);
          this.addNewRow();
        });
      this.productDialogOnHide();
    }
  }
  //#endregion

  //#region Calculation
  quantityOnFocus(orderLine: FormGroup): void {
    this._quantity = +orderLine.get('quantity').value;
  }

  quantityOnBlur(orderLine: FormGroup): void {
    let quantity = +orderLine.get('quantity').value;
    const price = +orderLine.get('price').value;
    const total = +orderLine.get('total').value;
    const currencyTotal = +orderLine.get('currencyTotal').value || null;

    if (quantity <= 0) {
      setTimeout(() => orderLine.get('quantity').patchValue(0), 0); quantity = 0;

      orderLine.get('price').patchValue(0);
      orderLine.get('vatAmount').patchValue(0);

      this.clearDiscounts(orderLine);
      this.clearCurrency(orderLine);
    }

    const hasChange = this.hasChange(this._quantity, quantity);
    if (this.hasProductCode(orderLine) && hasChange) {
      const unitGroupCode = <string>orderLine.get('unitGroupCode').value || null;

      if (price > 0) {
        orderLine.get('total').patchValue(0);
        orderLine.get('currencyTotal').patchValue(null);
      }

      if (quantity > 0 && quantity % 1 !== 0 && !isNullOrUndefined(unitGroupCode)) {
        this.gettingData = true;

        this.getUnitGroupByCode(unitGroupCode).subscribe(response => {
          const unitGroup = <UnitGroupWithDetailsDto>response.unit!.unitGroup;
          const unit = unitGroup.units.find(f => f.code === orderLine.get('unitCode').value);
          if (!unit.divisible)
            orderLine.get('quantity').patchValue(Math.round(quantity));

          if (price > 0) {
            setTimeout(() => {
              const total = +orderLine.get('total').value;
              const currencyTotal = +orderLine.get('currencyTotal').value || null;

              orderLine.get('total').patchValue(0);
              orderLine.get('currencyTotal').patchValue(null);

              this.calculate(() => {
                orderLine.get('total').patchValue(total);
                orderLine.get('currencyTotal').patchValue(currencyTotal);
              });
            }, 0);
          }
        });
      }
      else if (hasChange && price > 0 || total > 0) {
        setTimeout(() => {
          this.calculate(() => {
            orderLine.get('quantity').patchValue(this._quantity);
            orderLine.get('total').patchValue(total);
            orderLine.get('currencyTotal').patchValue(currencyTotal);
          });
        }, 0);
      }
    }
  }

  priceOnFocus(orderLine: FormGroup): void {
    this._price = +orderLine.get('price').value;
  }

  priceOnBlur(orderLine: FormGroup): void {
    let price = +orderLine.get('price').value;
    const total = +orderLine.get('total').value;
    const quantity = +orderLine.get('quantity').value;

    if (price <= 0) {
      setTimeout(() => orderLine.get('price').patchValue(0), 0); price = 0;

      this.clearDiscounts(orderLine);
      this.clearCurrency(orderLine);
    }

    const hasChange = this.hasChange(this._price, price);

    if (hasChange && this.hasProductCode(orderLine)) {
      if (quantity <= 0 && total > 0) {
        setTimeout(() => {
          this.calculateAggregateRoot().subscribe(response => {
            this.orderForm.patchValue({ ...response });
            this.checkDivisible(orderLine);
          });
        }, 0);
      }
      else if (quantity > 0) {
        orderLine.get('total').patchValue(0);
        setTimeout(() => {
          this.calculate(() => {
            orderLine.get('total').patchValue(total);
            orderLine.get('price').patchValue(this._price);
          });
        }, 0);
      }
    }
  }

  vatRateOnFocus(orderLine: FormGroup): void {
    this._vatRate = +orderLine.get('vatRate')?.value;
  }

  vatRateOnBlur(orderLine: FormGroup): void {
    let vatRate = +orderLine.get('vatRate').value;
    const total = +orderLine.get('total').value;
    const quantity = +orderLine.get('quantity').value;
    const price = +orderLine.get('price').value;

    if (vatRate <= 0) {
      orderLine.get('vatRate').patchValue(vatRate); vatRate = 0;
    }

    const hasChange = this.hasChange(this._vatRate, vatRate);
    if (
      hasChange &&
      this.hasProductCode(orderLine) &&
      total > 0 &&
      (quantity > 0 || price)
    )
      this.calculate();
  }

  isVatIncludedOnChange(orderLine: FormGroup): void {
    const vatRate = +orderLine.get('vatRate').value;
    const total = +orderLine.get('total').value;

    if (vatRate > 0 && total) this.calculate();
  }

  totalOnFocus(orderLine: FormGroup): void {
    this._total = +orderLine.get('total')?.value;
  }

  totalOnBlur(orderLine: FormGroup): void {
    const quantity = +orderLine.get('quantity').value;
    const price = +orderLine.get('price').value;
    let total = +orderLine.get('total').value;

    const hasChange = this.hasChange(this._total, total);

    if (total <= 0) {
      if (hasChange) {
        orderLine.get('price').patchValue(0);
        orderLine.get('quantity').patchValue(0);
      }

      setTimeout(() => orderLine.get('total').patchValue(0), 0);
      total = 0;
      this.clearDiscounts(orderLine);
      this.clearCurrency(orderLine);
    }

    if (hasChange) {
      if (quantity <= 0 && price <= 0) return;

      if (this.hasProductCode(orderLine)) {
        setTimeout(() => {
          this
            .calculateAggregateRoot(() => setTimeout(() =>
              orderLine.get('total').patchValue(this._total), 0
            ))
            .subscribe(response => {
              this.orderForm.patchValue({ ...response });
              this.checkDivisible(orderLine);
            });
        }, 0);
      }
    }
  }
  //#endregion

  //#region Units
  onDropdownShow(orderLine: FormGroup): void {
    const unitGroupCode = <string>orderLine.get('unitGroupCode')?.value;

    if (unitGroupCode) {
      this.gettingData = true;

      this.getUnitGroupByCode(unitGroupCode)
        .subscribe(state => {
          const unitsInState = <UnitDto>state.unit!.unitGroup!.units;
          orderLine.get('units').patchValue(unitsInState);
          this.cdr.detectChanges();
        });
    }
  }

  onDropdownHide(orderLine: FormGroup): void {
    if (+(<UnitDto[]>orderLine.get('units').value).length > 0) {
      const unitCode = <string>orderLine.get('unitCode').value;

      const units = <UnitDto[]>orderLine.get('units').value;
      const unit = units.find(f => f.code === unitCode);
      const quantity = +orderLine.get('quantity').value;

      orderLine.setControl('unit', this.fb.control({ ...unit }));
      orderLine.get('units').patchValue([]);

      if (!unit.divisible && quantity % 1 !== 0) {
        orderLine.get('quantity').patchValue(Math.round(quantity));
        this.calculate();
      }
    }
  }
  //#endregion

  //#region Discount
  discountTotalOnFocus(orderLine: FormGroup): void {
    this._discountTotal = +orderLine.get('discountTotal').value;
  }

  discountTotalOnBlur(orderLine: FormGroup): void {
    const total = +orderLine.get('total')?.value;
    const quantity = +orderLine.get('quantity')?.value
    const price = +orderLine.get('price')?.value;
    let discountTotal = +orderLine.get('discountTotal')?.value;

    if (discountTotal <= 0 || (quantity <= 0 && price <= 0) || total <= 0) {
      setTimeout(() => orderLine.get('discountTotal').patchValue(0), 0);
      orderLine.setControl('discounts', this.fb.array([]));
    }

    const hasChange = this.hasChange(this._discountTotal, discountTotal);
    if (hasChange && total > 0 && (quantity > 0 || price > 0)) {
      if (discountTotal > total) discountTotal = total;

      if (discountTotal > 0)
        orderLine.setControl(
          'discounts', this.fb.array([this.fb.group({ rate: 0, total: discountTotal })])
        );

      this.calculate();
    }
  }

  editDiscounts(orderLine: FormGroup, index: number): void {
    const orderForm = this.orderForm.getRawValue()!;
    const lines = this.orderLines.value.filter(f =>
      !isNullOrUndefined(f.productCode || null)
    );

    this.productAggregateRootDto = {
      currencyCode: <string>orderForm.currencyCode || null,
      currencyRate: +orderForm.currencyRate || null,
      totalDiscount: +orderForm.totalDiscount,
      totalVatBase: +orderForm.totalVatBase,
      totalVatAmount: +orderForm.totalVatAmount,
      totalGross: +orderForm.totalGross,
      discounts: <DiscountDto[]>this.orderDiscounts.value,
      lines: lines
    };

    this.selectedIndex = index;
    this._selectedRow = orderLine;

    this.discountDialogVisible = true;
  }

  handleDiscounts(
    productAggregateRootDto: CalculableProductAggregateRootDto
  ): void {
    this.orderForm.patchValue({ ...productAggregateRootDto });
    this._selectedRow.setControl(
      'discounts', this.fb.array(
        productAggregateRootDto.lines[this.selectedIndex]!.discounts?.map(
          discount => this.fb.group({ ...discount })
        )
      )
    );

    this.discountDialogVisible = false;
    this._selectedRow = undefined;
    this.selectedIndex = -1;
    this.productAggregateRootDto = undefined;
  }
  //#endregion

  //#region Deduction
  showDeductionList(orderLine: FormGroup): void {
    this._selectedRow = orderLine;
    this.deductionListSelectDialogVisible = true;
  }

  handleDeduction(deduction: DeductionDto): void {
    const selectedRow = this._selectedRow!;
    selectedRow.get('deductionCode').patchValue(<string>deduction.deductionCode);
    selectedRow.get('deductionPart1').patchValue(+deduction.deductionPart1);
    selectedRow.get('deductionPart2').patchValue(+deduction.deductionPart2);

    const total = +selectedRow.get('total').value;
    const vatAmount = +selectedRow.get('vatAmount').value;
    if (total > 0 && vatAmount > 0) this.calculate();

    this._selectedRow = undefined;
    this.deductionListSelectDialogVisible = false;
  }

  deductionCodeOnFocus(orderLine: FormGroup): void {
    this._deductionCode = <string>orderLine.get('deductionCode').value || null;
  }

  deductionCodeOnBlur(orderLine: FormGroup): void {
    let deductionCode = <string>orderLine.get('deductionCode').value || null;
    const deductionPart1 = +orderLine.get('deductionPart1').value || null;
    const deductionPart2 = +orderLine.get('deductionPart2').value || null;

    if (isNullOrUndefined(deductionCode) || isEmptyOrSpaces(deductionCode)) {
      orderLine.get('deductionCode').patchValue(null);
      deductionCode = null;
      orderLine.get('deductionPart1').patchValue(null);
      orderLine.get('deductionPart2').patchValue(null);
    }

    if (this._deductionCode !== deductionCode && deductionPart1 > 0 && deductionPart2 > 0)
      this.calculate(
        () => orderLine.get('deductionCode').patchValue(this._deductionCode)
      );
  }

  deductionPart1OnFocus(orderLine: FormGroup): void {
    this._deductionPart1 = +orderLine.get('deductionPart1').value || null;
  }

  deductionPart1OnBlur(orderLine: FormGroup): void {
    const code = <string>orderLine.get('deductionCode').value || null;
    const part1 = +orderLine.get('deductionPart1').value || null;
    const part2 = +orderLine.get('deductionPart2').value || null;

    if (this._deductionPart1 !== part1 && isNullOrUndefined(part1)) {
      orderLine.get('deductionCode').patchValue(null);
      orderLine.get('deductionPart2').patchValue(null);
    }

    if (
      this._deductionPart1 !== part1 &&
      (this.validateDeduction(code, part1, part2) || (!isNullOrUndefined(code) && part2 > 0))
    ) {
      this.calculate(() =>
        orderLine.get('deductionPart1').patchValue(
          this._deductionPart1 || (part2 > 0 ? part2 - 1 : null)
        )
      );
    }
  }

  deductionPart2OnFocus(orderLine: FormGroup): void {
    this._deductionPart2 = +orderLine.get('deductionPart2').value || null;
  }

  deductionPart2OnBlur(orderLine: FormGroup): void {
    const code = <string>orderLine.get('deductionCode').value || null;
    const part1 = +orderLine.get('deductionPart1').value || null;
    let part2 = +orderLine.get('deductionPart2').value || null;

    if (this._deductionPart2 !== part2 && (isNullOrUndefined(part2) || part2 <= 0)) {
      part2 = null;
      orderLine.get('deductionCode').patchValue(null);
      orderLine.get('deductionPart1').patchValue(null);
    }

    if (this._deductionPart2 !== part2 &&
      (this.validateDeduction(code, part1, part2) || (!isNullOrUndefined(code) && part1 > 0))
    ) {
      this.calculate(() =>
        orderLine.get('deductionPart1').patchValue(
          this._deductionPart1 || (part2 > 0 ? part2 - 1 : null)
        )
      );
    }
  }
  //#endregion

  //#region Currency
  currencyOnShow(orderLine: FormGroup): void {
    this._currencyCode = <string>orderLine.get('currencyCode').value || null;

    if (this.currencies.value.length < 1)
      this.store.dispatch(new GetCurrencyDailyExchanges(
        moment(this.orderForm.get('date')?.value).format('YYYY-MM-DD')
      ));
  }

  currencyOnHide(orderLine: FormGroup): void {
    const code = <string>orderLine.get('currencyCode').value || null;
    const currency = this.currencies.value.find(f => f.currencyCode === code);
    let rate = +orderLine.get('currencyRate').value;

    if (this._currencyCode !== code) {
      if (isNullOrUndefined(code)) {
        orderLine.get('currencyRate').patchValue(null);
      }
      else {
        orderLine.get('currencyRate').patchValue(
          this.orderType === OrderType.Purchase ?
            (currency.rate1 > 0 ? currency.rate1 : null) :
            (currency.rate2 > 0 ? currency.rate2 : null)
        );

        rate = +orderLine.get('currencyRate').value || null;

        if (rate <= 0) {
          this.confirmation.info(
            '::CurrencyRateNotFoundMessage', '::InformationTitleMessage',
            {
              messageLocalizationParams: [code],
              hideCancelBtn: true,
              yesText: 'AbpUi::Ok'
            }
          );
          orderLine.get('currencyCode').patchValue(null);
        }

        this.toggleCurrencyRatePriceAndTotal(orderLine, rate <= 0 ? false : true);
        this.togglePriceAndTotal(orderLine, rate <= 0 ? true : false);
      }

      this.clearCurrencPriceAndTotal(orderLine);
      this.calculate();
    }
  }

  currencyOnClear(orderLine: FormGroup): void {
    this.clearCurrency(orderLine);
    this.toggleCurrencyRatePriceAndTotal(orderLine);
    this.togglePriceAndTotal(orderLine, true);
    // this.calculate();
  }

  currencyRateOnFocus(orderLine: FormGroup): void {
    this._currencyRate = +orderLine.get('currencyRate').value || null;
  }

  currencyRateOnBlur(orderLine: FormGroup): void {
    const rate = +orderLine.get('currencyRate').value || null;
    const currencyPrice = +orderLine.get('currencyPrice').value;
    const currencyTotal = +orderLine.get('currencyTotal').value;

    if (isNullOrUndefined(rate) || rate <= 0) {
      orderLine.get('currencyRate').patchValue(null);
      orderLine.get('currencyCode').patchValue(null);

      this.toggleCurrencyRatePriceAndTotal(orderLine);
      this.togglePriceAndTotal(orderLine, true);
    }

    if (this._currencyRate !== rate) {
      orderLine.get('currencyPrice').patchValue(null);
      orderLine.get('currencyTotal').patchValue(null);

      if (rate > 0)
        this.calculate(
          () => {
            orderLine.get('currencyRate').patchValue(this._currencyRate);
            orderLine.get('currencyPrice').patchValue(currencyPrice);
            orderLine.get('currencyTotal').patchValue(currencyTotal);
          },
          () => {
            this._currencyPrice = orderLine.get('currencyPrice').value;
            this._currencyTotal = orderLine.get('currencyTotal').value;
          }
        );
    }
  }

  currencyPriceOnFocus(orderLine: FormGroup): void {
    this._currencyPrice = +orderLine.get('currencyPrice').value || null;
  }

  currencyPriceOnBlur(orderLine: FormGroup): void {
    const currencyPrice = +orderLine.get('currencyPrice').value?.toFixed(5) || null;
    const currencyTotal = +orderLine.get('currencyTotal').value || null;
    const price = +orderLine.get('price').value;
    const total = +orderLine.get('total').value;

    //#region Reset if null or less then zero or equal
    if (isNullOrUndefined(currencyPrice) || currencyPrice <= 0) {
      orderLine.get('currencyPrice').patchValue(null);
      orderLine.get('currencyCode').patchValue(null);
      orderLine.get('currencyRate').patchValue(null);

      this.toggleCurrencyRatePriceAndTotal(orderLine);
      this.togglePriceAndTotal(orderLine, true);
    }
    //#endregion

    const hasChange = +this._currencyPrice?.toFixed(5) !== currencyPrice;
    if (hasChange) {
      orderLine.get('currencyTotal').patchValue(null);

      if (currencyPrice > 0) {
        orderLine.get('price').patchValue(0);
        orderLine.get('total').patchValue(0);

        this.calculate(() => {
          orderLine.get('currencyPrice').patchValue(this._currencyPrice);
          orderLine.get('currencyTotal').patchValue(currencyTotal);
          orderLine.get('price').patchValue(price);
          orderLine.get('total').patchValue(total);
        },
          () => this._currencyTotal = orderLine.get('currencyTotal').value
        );
      }
    }
  }

  currencyTotalOnFocus(orderLine: FormGroup): void {
    this._currencyTotal = +orderLine.get('currencyTotal').value || null;
  }

  currencyTotalOnBlur(orderLine: FormGroup): void {
    const currencyTotal = +orderLine.get('currencyTotal').value || null;

    if (isNullOrUndefined(currencyTotal) || currencyTotal <= 0) {
      orderLine.get('currencyTotal').patchValue(null);
      orderLine.get('currencyPrice').patchValue(null);
      orderLine.get('currencyCode').patchValue(null);
      orderLine.get('currencyRate').patchValue(null);

      this.toggleCurrencyRatePriceAndTotal(orderLine);
      this.togglePriceAndTotal(orderLine, true);
    }

    const hasChange = +this._currencyTotal?.toFixed(5) !== currencyTotal;
    if (hasChange) {
      const price = +orderLine.get('price').value;
      const total = +orderLine.get('total').value;
      const currencyPrice = +orderLine.get('price').value;

      orderLine.get('currencyPrice').patchValue(null);
      orderLine.get('price').patchValue(0);
      orderLine.get('total').patchValue(0);

      if (currencyTotal > 0)
        this.calculate(
          () => {
            orderLine.get('currencyTotal').patchValue(this._currencyTotal);
            orderLine.get('currencyPrice').patchValue(currencyPrice);
            orderLine.get('price').patchValue(price);
            orderLine.get('total').patchValue(total);
          }
        );
    }

  }

  private toggleCurrencyRatePriceAndTotal(orderLine: FormGroup, enable: boolean = false): void {
    if (enable) {
      orderLine.get('currencyRate').enable()
      orderLine.get('currencyPrice').enable();
      orderLine.get('currencyTotal').enable();
    }
    else {
      orderLine.get('currencyRate').disable()
      orderLine.get('currencyPrice').disable();
      orderLine.get('currencyTotal').disable();
    }
  }

  private togglePriceAndTotal(orderLine: FormGroup, enable: boolean = false): void {
    if (enable) {
      orderLine.get('price').enable();
      orderLine.get('total').enable();
    }
    else {
      orderLine.get('price').disable();
      orderLine.get('total').disable();
    }
  }
  //#endregion

  //#region Reserve
  reserveDateOnFocus(orderLine: FormGroup): void {
    this._reserveDate = <Date>orderLine.get('reserveDate').value;
  }

  reserveDateAfterClose(orderLine: FormGroup): void {
    const reserveDate = <Date>orderLine.get('reserveDate').value;

    if (this._reserveDate !== reserveDate) {
      if (!isNullOrUndefined(reserveDate))
        orderLine.get('reserveQuantity').enable();
      else {
        orderLine.get('reserveQuantity').patchValue(null);
        orderLine.get('reserveQuantity').disable();
      }
    }
    this._reserveDate = undefined;
  }
  //#endregion

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
  //#endregion
}
