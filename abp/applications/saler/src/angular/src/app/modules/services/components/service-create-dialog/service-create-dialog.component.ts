import {
  Component,
  OnInit,
  EventEmitter,
  Input,
  Output,
  ViewChild,
  OnDestroy
} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Select, Store } from '@ngxs/store';
import { Dialog } from 'primeng/dialog';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CreateService } from '../../actions';
import { UnitGroupDto } from '@proxy/units';
import { ClearSelectedUnitGroup } from 'src/app/modules/units/actions';

import { DeductionDto } from '@proxy/calculations/product';
import { ServiceCreateDto } from '@proxy/services';

import * as serviceConsts from '../../consts';
import * as deductionConsts from 'src/app/modules/product-calculations/consts';

const { maxLength, required, min } = Validators;

@Component({
  selector: 'app-service-create-dialog',
  templateUrl: './service-create-dialog.component.html'
})
export class ServiceCreateDialogComponent implements OnInit, OnDestroy {
  //#region Fields
  private destroy$: Subject<void> = new Subject<void>();

  @Input() position: string = 'top';

  @Input() visible: boolean = false;
  @Output() visibleChange = new EventEmitter<boolean>();

  @Select(state => state.service!.busy) loading: Observable<boolean>;
  @Select(state => state.unit!.selectedUnitGroup) selectedUnitGroup: Observable<UnitGroupDto>;

  @ViewChild('serviceCreateDialog') dialog: Dialog;

  serviceForm!: FormGroup;
  unitGroupListSelectDialogVisible: boolean = false;
  deductionListSelectDialogVisible: boolean = false;
  //#endregion

  //#region Utilities
  private buildForm(): void {
    this.serviceForm = this.fb.group({
      code: ['', [required, maxLength(serviceConsts.maxCodeLength)]],
      name: ['', maxLength(serviceConsts.maxCodeLength)],
      unitGroupCode: ['', required],
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
          this.serviceForm.patchValue({ unitGroupCode: response.code });

        this.unitGroupListSelectDialogVisible = false;
      });
  }

  handleDeduction(deduction: DeductionDto): void {
    const serviceForm = this.serviceForm!;
    serviceForm.get('deduction.code').patchValue(deduction?.deductionCode);
    serviceForm.get('deduction.salesPart1').patchValue(deduction?.deductionPart1);
    serviceForm.get('deduction.salesPart2').patchValue(deduction?.deductionPart2);
    this.deductionListSelectDialogVisible = false;
  }

  save(): void {
    const serviceForm = this.serviceForm!.value!;
    const deduction = serviceForm.deduction;

    const input = <ServiceCreateDto>{
      code: <string>serviceForm.code,
      name: <string>serviceForm.name,
      unitGroupCode: <string>serviceForm.unitGroupCode,
      purchaseVatRate: +serviceForm.purchaseVatRate,
      salesVatRate: +serviceForm.salesVatRate,
      deductionCode: deduction?.code || null,
      salesDeductionPart1: deduction?.salesPart1 || null,
      salesDeductionPart2: deduction?.salesPart2 || null,
      purchaseDeductionPart1: deduction?.purchasePart1 || null,
      purchaseDeductionPart2: deduction?.purchasePart2 || null
    };

    this.store
      .dispatch(new CreateService(input))
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.hideDialog());
  }

  unitGroupListDialogOnHide = (): any => this.store.dispatch(new ClearSelectedUnitGroup())

  hideDialog(): void {
    this.visible = false;
    this.visibleChange.emit(this.visible);
  }

  ngOnDestroy(): void {
    this.store.dispatch(new ClearSelectedUnitGroup());
    this.serviceForm.reset();
    this.destroy$.next();
    this.destroy$.complete();
  }
  //#endregion
}
