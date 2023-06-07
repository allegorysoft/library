import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { takeUntil } from 'rxjs/operators';
import { Observable, Subject } from 'rxjs';

import { Select, Store } from '@ngxs/store';

import * as clientConsts from '../../consts';
import { ClearClient, GetClient, UpdateClient } from '../../actions';
import { ClientDto } from '@proxy/clients';

const { maxLength, required } = Validators;

@Component({
  selector: 'app-client-edit-dialog',
  templateUrl: './client-edit-dialog.component.html',
  styleUrls: ['./client-edit-dialog.component.scss']
})
export class ClientEditDialogComponent implements OnInit, OnDestroy {
  //#region Fields
  destroy$: Subject<void> = new Subject<void>();
  clientForm!: FormGroup;
  @Input() visible: boolean = false;
  @Input() position: string = 'top';
  @Input() clientId: number;

  @Output() visibleChange = new EventEmitter<boolean>();

  @Select(state => state.client!.saving) loading: Observable<boolean>;
  //#endregion

  //#region Utilities
  private buildForm(data: ClientDto): void {
    this.clientForm = this.fb.group({
      code: [data?.code, [required, maxLength(clientConsts.maxCodeLength)]],
      title: [data?.title, maxLength(clientConsts.maxTitleLength)],
      identityNumber: [data?.identityNumber, maxLength(clientConsts.maxIdentityNumberLength)]
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
    if (!this.clientId) {
      this.hideDialog();
    }

    this.store
      .dispatch(new GetClient(this.clientId))
      .pipe(takeUntil(this.destroy$))
      .subscribe(state => this.buildForm(state.client!.client as ClientDto));
  }

  save(): void {
    this.store
      .dispatch(new UpdateClient(this.clientId, this.clientForm.value))
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.hideDialog());
  }

  hideDialog(): void {
    this.visible = false;
    this.visibleChange.emit(this.visible);
  }

  ngOnDestroy(): void {
    this.clientForm.reset();
    this.store.dispatch(new ClearClient());
    this.destroy$.next();
    this.destroy$.complete();
  }
  //#endregion
}
