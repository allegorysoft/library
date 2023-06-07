import {
  Component,
  EventEmitter,
  Inject,
  Input,
  OnDestroy,
  OnInit,
  Output
} from '@angular/core';
import { Observable, of, Subject } from 'rxjs';
import { finalize, takeUntil } from 'rxjs/operators';
import { OBSERVE, Observed, ObserveFn, OBSERVE_PROVIDER } from 'ng-observe';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { isNullOrUndefined, PermissionService } from '@abp/ng.core';
import { Select, Store } from '@ngxs/store';
import { UnitPriceType, UnitPriceWithDetailsDto } from '@proxy/unit-prices';
import { FilteredPagedAndSortedResultRequestDto } from '@proxy';
import { CurrencyDto } from '@proxy/currencies';
import { ClientDto } from '@proxy/clients';
import { ItemWithDetailsDto } from '@proxy/items';
import { ServiceWithDetailsDto } from '@proxy/services';
import { UnitDto, UnitGroupWithDetailsDto } from '@proxy/units';
import { ClearUnitPrice, CreateUnitPrice, UpdateUnitPrice } from '../../actions';
import { GetCurrencies } from 'src/app/modules/currencies/actions';
import { UnitPriceState } from '../../states';
import { ClearItem, GetItemByCode } from 'src/app/modules/items/actions';
import { ClearService, GetServiceByCode } from 'src/app/modules/services/actions';
import { ClearUnitGroup, GetUnitGroupByCode } from 'src/app/modules/units/actions';
import * as unitPriceConsts from '../../consts';
import * as moment from 'moment';

const { required, maxLength } = Validators;

@Component({
  selector: 'app-unit-price-edit-dialog',
  templateUrl: './unit-price-edit-dialog.component.html',
  providers: [OBSERVE_PROVIDER]
})
export class UnitPriceEditDialogComponent implements OnInit, OnDestroy {
  //#region Fields
  private destroy$: Subject<void> = new Subject<void>();
  private _currencies: Observed<CurrencyDto[]>;

  @Input() type?: UnitPriceType;

  @Input() visible: boolean = false;
  @Output() visibleChange = new EventEmitter<boolean>();

  @Select(UnitPriceState.getUnitPrice) unitPrice$: Observable<UnitPriceWithDetailsDto>;
  @Select(UnitPriceState.getLoadingUnitPrice) loading$: Observable<boolean>;
  @Select(UnitPriceState.getBusy) busy$: Observable<boolean>;

  units: UnitDto[];
  unitPriceForm!: FormGroup;
  hasCreateOrEditPermission$: Observable<boolean> = of(false);
  dialogVisible: boolean = false;//Item or service
  clientDialogVisible: boolean = false;
  //#endregion

  //#region Utilities
  private buildForm(unitPrice?: UnitPriceWithDetailsDto): void {
    this.unitPriceForm = this.fb.group({
      type: [unitPrice?.type || this.type],
      code: [unitPrice?.code || '', [required, maxLength(unitPriceConsts.maxCodeLength)]],
      productCode: [unitPrice?.productCode || '', required],
      productName: [unitPrice?.productName || ''],
      unitCode: [unitPrice?.unitCode || '', required],
      currencyCode: [unitPrice?.currencyCode || null],
      salesPrice: [unitPrice?.salesPrice || 0],
      purchasePrice: [unitPrice?.purchasePrice || 0],
      beginDate: [
        unitPrice?.beginDate ? new Date(unitPrice.beginDate) : null,
        required
      ],
      endDate: [
        unitPrice?.endDate ? new Date(unitPrice.endDate) : null,
        required
      ],
      isVatIncluded: [unitPrice?.isVatIncluded || false, required],
      clientCode: [unitPrice?.clientCode || null]
    });

    if (unitPrice?.id > 0)
      this.unitPriceForm.addControl('id', this.fb.control(unitPrice.id));

    if (unitPrice?.productCode)
      this.getProductByCode(unitPrice.productCode);
  }

  private getProductByCode(code: string): void {
    switch (<UnitPriceType>this.unitPriceForm.get('type').value) {
      case UnitPriceType.Item:
        this.store.dispatch(new GetItemByCode(code))
          .pipe(
            takeUntil(this.destroy$),
            finalize(() => this.store.dispatch(new ClearItem()))
          )
          .subscribe(state => {
            const item = <ItemWithDetailsDto>state!.item!.item;
            this.getUnitGroupByCode(item.unitGroupCode);
          });
        break;
      case UnitPriceType.Service:
        this.store.dispatch(new GetServiceByCode(code))
          .pipe(
            takeUntil(this.destroy$),
            finalize(() => this.store.dispatch(new ClearService()))
          )
          .subscribe(state => {
            const service = <ServiceWithDetailsDto>state!.service!.service;
            this.getUnitGroupByCode(service.unitGroupCode);
          });
        break;
    }
  }

  private getUnitGroupByCode(code: string): void {
    this.store.dispatch(new GetUnitGroupByCode(code))
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.store.dispatch(new ClearUnitGroup()))
      )
      .subscribe(state => {
        const unitGroup = <UnitGroupWithDetailsDto>state!.unit!.unitGroup;
        this.units = unitGroup.units;
      });
  }

  private checkPermission(): void {
    this.hasCreateOrEditPermission$ = this.permissionService.getGrantedPolicy$(
      'ProductManagement.UnitPrice.Create || ProductManagement.UnitPrice.Edit'
    );
  }

  private loadCurrencies(): void {
    //TODO: Refactor maxResultCount value
    this.store.dispatch(new GetCurrencies(
      <FilteredPagedAndSortedResultRequestDto>{
        maxResultCount: 1000
      }
    ));
  }

  get currencies(): CurrencyDto[] {
    return this._currencies.value;
  }

  get dialogTitle(): string {
    return this.type == UnitPriceType.Item ? '::SelectItem' : '::SelectService';
  }
  //#endregion

  //#region Ctor
  constructor(
    @Inject(OBSERVE) private observe: ObserveFn,
    private fb: FormBuilder,
    private store: Store,
    private readonly permissionService: PermissionService
  ) {
    this._currencies = this.observe(this.store.select(state => state!.currency!.currencies));
  }
  //#endregion

  //#region Methods
  ngOnInit(): void {
    this.checkPermission();

    this.loadCurrencies();

    this.unitPrice$
      .pipe(takeUntil(this.destroy$))
      .subscribe(response => {
        this.buildForm(response);
        if (isNullOrUndefined(this.type)) this.type = response.type;
      });
  }

  productDialogOnHide(ev?: any): void {
    if (!isNullOrUndefined(ev)) {
      this.unitPriceForm.get('productCode').patchValue(ev.code);
      this.unitPriceForm.get('productName').patchValue(ev.name);
      this.unitPriceForm.get('unitCode').patchValue(ev.mainUnitCode);
      this.getProductByCode(ev.code);
    }
    this.dialogVisible = false;
  }

  clientAfterSelect(client: ClientDto): void {
    if (!isNullOrUndefined(client))
      this.unitPriceForm.get('clientCode').patchValue(client.code);

    this.clientDialogVisible = false;
  }

  hideDialog(): void {
    this.visible = false;
    this.visibleChange.emit(this.visible);
  }

  save(): void {
    if (this.unitPriceForm.invalid) return;

    const input = this.unitPriceForm.value;
    const { id } = input;

    input.beginDate = moment(input.beginDate).format('YYYY-MM-DDTHH:mm:ss');
    input.endDate = moment(input.endDate).format('YYYY-MM-DDTHH:mm:ss');

    this.store
      .dispatch(
        id
          ? new UpdateUnitPrice(id, input)
          : new CreateUnitPrice(input)
      )
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.hideDialog());
  }

  ngOnDestroy(): void {
    this.store.dispatch(new ClearUnitPrice());
    this.unitPriceForm.reset();
    this.destroy$.next();
    this.destroy$.complete();
  }
  //#endregion
}
