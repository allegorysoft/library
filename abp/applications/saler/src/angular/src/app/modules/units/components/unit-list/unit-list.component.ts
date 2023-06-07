import {
  Component,
  OnInit,
  Input,
  OnDestroy
} from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { GlobalUnitDto, UnitCreateOrUpdateDtoBase } from '@proxy/units';
import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { isNullOrUndefined } from '@abp/ng.core';
import { isEmptyOrSpaces } from 'src/app/shared/utils';

@Component({
  selector: 'app-unit-list',
  templateUrl: './unit-list.component.html',
  styleUrls: ['./unit-list.component.scss']
})
export class UnitListComponent implements OnInit, OnDestroy {
  //#region Fields
  destroy$: Subject<void> = new Subject<void>();
  @Input() unitGroupForm!: FormGroup;
  minFractionDigits: number = 0;
  maxFractionDigits: number = 5;
  globalUnitDialogVisible: boolean = false;
  selectedUnit!: FormGroup;
  //#endregion

  //#region Utilities
  private addRow(unit: UnitCreateOrUpdateDtoBase): void {
    this.units.push(this.fb.group({
      id: [unit?.id || null],
      code: [unit?.code],
      name: [unit?.name],
      mainUnit: [unit?.mainUnit || false],
      divisible: [unit?.divisible || false],
      convFact1: [unit?.convFact1 || null],
      convFact2: [unit?.convFact2 || null],
      globalUnitCode: [unit?.globalUnitCode || null]
    }));
  }

  private checkDivisible(unit: FormGroup): void {
    const convFact1: number = <number>unit!.get('convFact1')?.value;
    const convFact2: number = <number>unit!.get('convFact2')?.value;
    const divisible: boolean = <boolean>unit!.get('divisible').value;

    if (!divisible && (convFact1 % 1 !== 0 || convFact2 % 1 !== 0)) {
      setTimeout(() => {
        unit.get('convFact1').patchValue(Math.round(convFact1));
        unit.get('convFact2').patchValue(Math.round(convFact2));
      }, 0);
    }
  }

  get units(): FormArray {
    return this.unitGroupForm!.get('units') as FormArray;
  }

  get newRowLength(): number {
    return this.units.controls.filter(f =>
      isEmptyOrSpaces(f.get('code').value) && !f.get('id')?.value
    ).length;
  }
  //#endregion

  //#region Ctor
  constructor(
    private readonly fb: FormBuilder,
    private readonly confirmation: ConfirmationService
  ) { }
  //#endregion

  //#region Methods
  ngOnInit(): void {
    this.createUnit();
  }

  createUnit(): void {
    if (this.newRowLength < 1) {
      this.addRow({
        code: '',
        name: '',
        mainUnit: false,
        convFact1: 1,
        convFact2: 1,
        divisible: false
      });
    }
  }

  globalUnitSelect(unit: FormGroup): void {
    this.selectedUnit = unit;
    this.globalUnitDialogVisible = true;
  }

  handleGlobalUnit(globalUnit: GlobalUnitDto): void {
    this.selectedUnit.get('globalUnitCode')?.patchValue(globalUnit.code);
    this.globalUnitDialogVisible = false;
    this.selectedUnit = undefined;
  }

  mainUnitSelectedChange(unit: FormGroup): void {
    this.units.controls
      .filter(f => f !== unit)
      .map(unit => unit.patchValue({ mainUnit: false }));
  }

  divsibileOnChange = (unit: FormGroup): any => this.checkDivisible(unit)

  convFactOnBlur = (unit: FormGroup): any => this.checkDivisible(unit)

  onBlur(unit: FormGroup): void {
    const code = unit?.get('code')?.value || null;
    if (!isNullOrUndefined(code) && !isEmptyOrSpaces(code)) {
      this.createUnit();
    }
  }

  removeUnit(index: number, unit: FormGroup): void {
    this.confirmation
      .warn('::UnitDeletionConfirmationMessage', '::AreYouSure', {
        messageLocalizationParams: [unit.get('name').value as string]
      })
      .pipe(takeUntil(this.destroy$))
      .subscribe(response => {
        if (response == Confirmation.Status.confirm) {
          this.units.removeAt(index);
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
  //#endregion
}
