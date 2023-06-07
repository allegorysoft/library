import { OnInit, Component, EventEmitter, Input, Output, OnDestroy } from '@angular/core';
import { isNullOrUndefined } from '@abp/ng.core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Select, Store } from '@ngxs/store';
import { UnitGroupWithDetailsDto } from '@proxy/units';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { isEmptyOrSpaces } from 'src/app/shared/utils';
import { GetUnitGroup, UpdateUnitGroup, ClearUnitGroup } from '../../actions';
import * as unitConsts from '../../consts';

const { maxLength, required } = Validators;

@Component({
  selector: 'app-unit-group-edit-dialog',
  templateUrl: './unit-group-edit-dialog.component.html',
  styleUrls: ['./unit-group-edit-dialog.component.scss']
})
export class UnitGroupEditDialogComponent implements OnInit, OnDestroy {
  //#region Fields
  destroy$: Subject<void> = new Subject<void>();
  unitGroupForm!: FormGroup;

  @Input() visible: boolean = false;
  @Input() position: string = 'top';
  @Input() unitGroupId: number;

  @Output() visibleChange = new EventEmitter<boolean>();
  @Select(state => state.unit!.busy) busy: Observable<boolean>;
  //#endregion

  //#region Utilities
  private buildForm(data: UnitGroupWithDetailsDto = null): void {
    this.unitGroupForm = this.fb.group({
      code: [data!.code, [required, maxLength(unitConsts.unitGroupCodeMaxLength)]],
      name: [data!.name, maxLength(unitConsts.unitGroupNameMaxLength)],
      extraProperties: [data?.extraProperties],
      units: this.fb.array(
        data!.units!.map(unit => this.fb.group({ ...unit }))
      )
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
    if (!this.unitGroupId) {
      this.hideDialog();
    }

    this.store
      .dispatch(new GetUnitGroup(this.unitGroupId))
      .pipe(takeUntil(this.destroy$))
      .subscribe(state => this.buildForm(state.unit.unitGroup as UnitGroupWithDetailsDto));
  }

  save(): void {
    const index = this.units.controls.findIndex(
      f => isNullOrUndefined(f.get('id')?.value) && isEmptyOrSpaces(f.get('code')?.value)
    );

    if (index > -1)
      this.units.removeAt(index);

    this.store
      .dispatch(new UpdateUnitGroup(this.unitGroupId, this.unitGroupForm.value))
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.hideDialog())
  }

  hideDialog(): void {
    this.visible = false;
    this.visibleChange.emit(this.visible);
  }

  ngOnDestroy(): void {
    this.unitGroupForm.reset();
    this.store.dispatch(new ClearUnitGroup());
    this.destroy$.next();
    this.destroy$.complete();
  }
  //#endregion
}
