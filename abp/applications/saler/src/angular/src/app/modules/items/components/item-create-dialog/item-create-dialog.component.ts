import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
  ViewChild
} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Select, Store } from '@ngxs/store';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CreateItem } from '../../actions';
import { ItemCreateDto, ItemType, itemTypeOptions } from '@proxy/items'

import { Dialog } from 'primeng/dialog';
import { UnitGroupDto } from '@proxy/units';
import { ClearSelectedUnitGroup } from 'src/app/modules/units/actions';
import { ABP } from '@abp/ng.core';

import { DeductionDto } from '@proxy/calculations/product';

import * as itemConsts from '../../consts';
import * as deductionConsts from 'src/app/modules/product-calculations/consts';

const { maxLength, required, min } = Validators;

@Component({
  selector: 'app-item-create-dialog',
  templateUrl: './item-create-dialog.component.html'
})
export class ItemCreateDialogComponent implements OnInit, OnDestroy {
  //#region Fields
  private destroy$: Subject<void> = new Subject<void>();

  @Input() position: string = 'top';

  @Input() visible: boolean = false;
  @Output() visibleChange = new EventEmitter<boolean>();

  @Select(state => state.item!.busy) busy$: Observable<boolean>;
  @Select(state => state.unit!.selectedUnitGroup) selectedUnitGroup: Observable<UnitGroupDto>;

  @ViewChild('itemCreateDialog') dialog: Dialog;

  itemForm!: FormGroup;
  itemTypes: ABP.Option<typeof ItemType>[] = itemTypeOptions;
  unitGroupListSelectDialogVisible: boolean = false;
  deductionListSelectDialogVisible: boolean = false;
  //#endregion

  //#region Utilities
  private buildForm(): void {
    this.itemForm = this.fb.group({
      code: ['', [required, maxLength(itemConsts.maxCodeLength)]],
      name: ['', maxLength(itemConsts.maxCodeLength)],
      unitGroupCode: ['', required],
      type: [0, required],
      purchaseVatRate: [0, [required, min(0)]],
      salesVatRate: [0, [required, min(0)]],
      deduction: this.fb.group({
        code: [null, maxLength(deductionConsts.maxDeductionCodeLength)],
        salesPart1: [null],
        salesPart2: [null],
        purchasePart1: [null],
        purchasePart2: [null]
      })
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
    this.buildForm();

    this.selectedUnitGroup
      .pipe(takeUntil(this.destroy$))
      .subscribe(response => {
        if (response)
          this.itemForm.patchValue({ unitGroupCode: response.code });

        this.unitGroupListSelectDialogVisible = false;
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
    const deduction = itemForm.deduction;
    const vatRate = itemForm.vatRate;

    const input: ItemCreateDto = <ItemCreateDto>{
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
      purchaseDeductionPart2: deduction?.purchasePart2 || null
    };

    this.store
      .dispatch(new CreateItem(input))
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.hideDialog());
  }

  hideDialog(): void {
    this.visible = false;
    this.visibleChange.emit(this.visible);
  }

  unitGroupListDialogOnHide = (): any => this.store.dispatch(new ClearSelectedUnitGroup())

  ngOnDestroy(): void {
    this.store.dispatch(new ClearSelectedUnitGroup());
    this.itemForm.reset();
    this.destroy$.next();
    this.destroy$.complete();
  }
  //#endregion
}
