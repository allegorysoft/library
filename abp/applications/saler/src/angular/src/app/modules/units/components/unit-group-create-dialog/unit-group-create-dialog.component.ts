import {
  Component,
  OnInit,
  Input,
  Output,
  EventEmitter,
  OnDestroy
} from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';

import { Select, Store } from '@ngxs/store';

import { CreateUnitGroup } from '../../actions';
import { takeUntil } from 'rxjs/operators';
import { Observable, Subject } from 'rxjs';
import { isNullOrUndefined } from '@abp/ng.core';
import { isEmptyOrSpaces } from 'src/app/shared/utils';
import * as unitConsts from '../../consts';

const { maxLength, required } = Validators;

@Component({
  selector: 'app-unit-group-create-dialog',
  templateUrl: './unit-group-create-dialog.component.html',
  styleUrls: ['./unit-group-create-dialog.component.scss']
})
export class UnitGroupCreateDialogComponent implements OnInit, OnDestroy {
  //#region Fields
  destroy$: Subject<void> = new Subject<void>();
  unitGroupForm!: FormGroup;
  @Input() visible: boolean = false;
  @Input() position: string = 'top';

  @Output() visibleChange = new EventEmitter<boolean>();

  @Select(state => state.unit!.busy) busy: Observable<boolean>;
  //#endregion

  //#region Utilities
  private buildForm(): void {
    this.unitGroupForm = this.fb.group({
      code: ['', [required, maxLength(unitConsts.unitGroupCodeMaxLength)]],
      name: ['', maxLength(unitConsts.unitGroupNameMaxLength)],
      units: this.fb.array([])
    });
  }

  private get units(): FormArray {
    return this.unitGroupForm.get('units') as FormArray;
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
  }

  save(): void {
    const index = this.units.controls.findIndex(
      f => isNullOrUndefined(f.get('id')?.value) && isEmptyOrSpaces(f.get('code')?.value)
    );

    if (index > -1)
      this.units.removeAt(index);

    this.store
      .dispatch(new CreateUnitGroup(this.unitGroupForm.value))
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.hideDialog());
  }

  hideDialog(): void {
    this.visible = false;
    this.visibleChange.emit(this.visible);
  }

  ngOnDestroy(): void {
    this.unitGroupForm.reset();
    this.destroy$.next();
    this.destroy$.complete();
  }
  //#endregion
}
