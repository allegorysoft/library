import { Component, OnInit, EventEmitter, Input, Output, ViewChild, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Select, Store } from '@ngxs/store';
import { ItemCreateDto, ItemType, itemTypeOptions, ItemUpdateDto, ItemWithDetailsDto } from '@proxy/items';
import { UnitGroupDto } from '@proxy/units';
import { Dialog } from 'primeng/dialog';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ClearSelectedUnitGroup } from 'src/app/modules/units/actions';
import { ClearItem, GetItem, UpdateItem } from '../../actions';

import * as itemConsts from '../../consts';
import * as deductionConsts from 'src/app/modules/product-calculations/consts';
import { DeductionDto } from '@proxy/calculations/product';

const { maxLength, required, min } = Validators;

@Component({
  selector: 'app-item-edit-dialog',
  templateUrl: './item-edit-dialog.component.html'
})
export class ItemEditDialogComponent implements OnInit, OnDestroy {
  //#region Fields
  private destroy$: Subject<void> = new Subject<void>();

  @Input() position: string = 'top';
  @Input() itemId: number;

  @Input() visible: boolean = false;
  @Output() visibleChange = new EventEmitter<boolean>();

  @Select(state => state.unit!.selectedUnitGroup) selectedUnitGroup: Observable<UnitGroupDto>;
  @Select(state => state.item.busy) busy$: Observable<boolean>;

  @ViewChild('itemEditDialog') dialog: Dialog;

  itemForm!: FormGroup;
  itemTypes = itemTypeOptions;
  unitGroupListSelectDialogVisible: boolean = false;
  deductionListSelectDialogVisible: boolean = false;
  //#endregion

  //#region Utilities
  private buildForm(data: ItemWithDetailsDto): void {
    this.itemForm = this.fb.group({
      code: [data?.code, [required, maxLength(itemConsts.maxCodeLength)]],
      name: [data?.name, maxLength(itemConsts.maxCodeLength)],
      unitGroupCode: [data?.unitGroupCode, required],
      purchaseVatRate: [data?.purchaseVatRate || 0, [required, min(0)]],
      salesVatRate: [data?.salesVatRate || 0, [required, min(0)]],
      deduction: this.fb.group({
        code: [data?.deductionCode || null, maxLength(deductionConsts.maxDeductionCodeLength)],
        salesPart1: [data?.salesDeductionPart1 || null],
        salesPart2: [data?.salesDeductionPart2 || null],
        purchasePart1: [data?.purchaseDeductionPart1 || null],
        purchasePart2: [data?.purchaseDeductionPart2 || null]
      }),
      extraProperties: [data?.extraProperties]
    });
  }
  //#endregion

  //#region Ctor
  constructor(
    private store: Store,
    private fb: FormBuilder
  ) { }
  //#endregion

  //#region Methods
  ngOnInit(): void {
    if (!this.itemId)
      this.hideDialog();

    this.store
      .dispatch(new GetItem(this.itemId))
      .pipe(takeUntil(this.destroy$))
      .subscribe(state => this.buildForm(state.item!.item as ItemWithDetailsDto));

    this.selectedUnitGroup
      .pipe(takeUntil(this.destroy$))
      .subscribe(response => {
        this.unitGroupListSelectDialogVisible = false;
        if (response)
          this.itemForm.patchValue({ unitGroupCode: response.code });
      });
  }

  handleDeduction(deduction: DeductionDto): void {
    const itemForm = this.itemForm!;
    itemForm.get('deduction.code').patchValue(deduction?.deductionCode);
    itemForm.get('deduction.salesPart1').patchValue(deduction?.deductionPart1);
    itemForm.get('deduction.salesPart2').patchValue(deduction?.deductionPart2);
    this.deductionListSelectDialogVisible = false;
  }

  save(): void {
    const itemForm = this.itemForm!.value!;
    const deduction = itemForm!.deduction;
    const vatRate = itemForm.vatRate;

    const input: ItemUpdateDto = <ItemUpdateDto>{
      id: +itemForm.id,
      type: <ItemType>itemForm.type,
      code: <string>itemForm.code,
      name: <string>itemForm.name,
      unitGroupCode: <string>itemForm.unitGroupCode,
      purchaseVatRate: +itemForm.purchaseVatRate,
      salesVatRate: +itemForm.salesVatRate,
      deductionCode: deduction?.code || null,
      salesDeductionPart1: deduction?.salesPart1 || null,
      salesDeductionPart2: deduction?.salesPart2 || null,
      purchaseDeductionPart1: deduction?.purchasePart1 || null,
      purchaseDeductionPart2: deduction?.purchasePart2 || null,
      extraProperties: itemForm.extraProperties || null,
    };

    this.store
      .dispatch(new UpdateItem(this.itemId, input))
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.hideDialog());
  }

  hideDialog(): void {
    this.visible = false;
    this.visibleChange.emit(this.visible);
  }

  unitGroupListDialogOnHide = (): any => this.store.dispatch(new ClearSelectedUnitGroup())

  ngOnDestroy(): void {
    this.store.dispatch([new ClearItem(), new ClearSelectedUnitGroup()]);
    this.itemForm.reset();
    this.destroy$.next();
    this.destroy$.complete();
  }
  //#endregion
}
