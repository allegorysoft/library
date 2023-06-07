import { Component, OnInit, EventEmitter, Input, Output, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Select, Store } from '@ngxs/store';
import { CurrencyCreateUpdateDto, CurrencyDto } from '@proxy/currencies';
import { Observable, Subject } from 'rxjs';
import { CurrencyState } from '../../states';
import * as currencyConsts from '../../consts';
import { takeUntil } from 'rxjs/operators';
import { CreateCurrency, UpdateCurrency, ClearCurrency } from '../../actions';
import { PermissionService } from '@abp/ng.core';

const { required, maxLength } = Validators;

@Component({
  selector: 'app-currency-edit-dialog',
  templateUrl: './currency-edit-dialog.component.html',
  styleUrls: ['./currency-edit-dialog.component.scss']
})
export class CurrencyEditDialogComponent implements OnInit, OnDestroy {
  //#region Fields
  private destroy$: Subject<void> = new Subject<void>();
  @Select(CurrencyState.getCurrency) private selectedCurrency$: Observable<CurrencyDto>;
  @Select(CurrencyState.getBusy) busy: Observable<boolean>;

  @Input() visible: boolean = false;
  @Output() visibleChange: EventEmitter<boolean> = new EventEmitter<boolean>();

  currencyForm!: FormGroup;
  hasCreateOrEditPermission$: Observable<boolean>;
  //#endregion

  //#region Utilities
  private buildForm(data?: CurrencyDto): void {
    this.currencyForm = this.fb.group({
      id: [data?.id || null],
      code: [data?.code, [required, maxLength(currencyConsts.maxCodeLength)]],
      name: [data?.name, [maxLength(currencyConsts.maxNameLength)]],
      symbol: [data?.symbol, [maxLength(currencyConsts.maxSymbolLength)]]
    });
  }
  //#endregion

  //#region Ctor
  constructor(
    private store: Store,
    private fb: FormBuilder,
    private readonly permissionService: PermissionService
  ) { }
  //#endregion

  //#region Methods
  ngOnInit(): void {
    this.hasCreateOrEditPermission$ = this.permissionService.getGrantedPolicy$(
      'General.Currency.Create || General.Currency.Edit'
    );
    this.selectedCurrency$
      .pipe(takeUntil(this.destroy$))
      .subscribe((response) => this.buildForm(response));
  }

  save(): void {
    const input: CurrencyCreateUpdateDto = {
      id: +this.currencyForm.value.id || null,
      code: <string>this.currencyForm.value.code,
      name: <string>this.currencyForm.value.name,
      symbol: <string>this.currencyForm.value.symbol
    };

    const req = this.store.dispatch(
      input.id > 0
        ? new UpdateCurrency(input.id, input)
        : new CreateCurrency(input)
    );

    req.pipe(takeUntil(this.destroy$)).subscribe(() => this.hideDialog());
  }

  hideDialog(): void {
    this.visible = false;
    this.visibleChange.emit(this.visible);
  }

  ngOnDestroy(): void {
    this.store.dispatch(new ClearCurrency());
    this.currencyForm.reset();
    this.destroy$.next();
    this.destroy$.complete();
  }
  //#endregion
}
