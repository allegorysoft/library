import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { takeUntil } from 'rxjs/operators';
import { Observable, Subject } from 'rxjs';

import { Select, Store } from '@ngxs/store';

import * as clientConsts from '../../consts';
import { CreateClient } from '../../actions';

const { maxLength, required } = Validators;

@Component({
  selector: 'app-client-create-dialog',
  templateUrl: './client-create-dialog.component.html',
  styleUrls: ['./client-create-dialog.component.scss']
})
export class ClientCreateDialogComponent implements OnInit, OnDestroy {
  //#region Fields
  destroy$: Subject<void> = new Subject<void>();
  clientForm!: FormGroup;
  @Input() visible: boolean = false;
  @Input() position: string = 'top';

  @Output() visibleChange = new EventEmitter<boolean>();

  @Select(state => state.client!.saving) loading: Observable<boolean>;
  //#endregion

  //#region Utilities
  private buildForm(): void {
    this.clientForm = this.fb.group({
      code: ['', [required, maxLength(clientConsts.maxCodeLength)]],
      title: ['', maxLength(clientConsts.maxTitleLength)],
      identityNumber: ['', maxLength(clientConsts.maxIdentityNumberLength)]
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
  }

  save(): void {
    this.store
      .dispatch(new CreateClient(this.clientForm.value))
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.hideDialog());
  }

  hideDialog(): void {
    this.visible = false;
    this.visibleChange.emit(this.visible);
  }

  ngOnDestroy(): void {
    this.clientForm.reset();
    this.destroy$.next();
    this.destroy$.complete();
  }
  //#endregion
}
