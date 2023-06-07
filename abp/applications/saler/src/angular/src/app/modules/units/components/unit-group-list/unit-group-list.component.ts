import { Component, OnInit, OnDestroy, ChangeDetectorRef, ViewChild } from '@angular/core';
import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';

import { Select, Store } from '@ngxs/store';
import { UnitState } from '../../states';

import { UnitGroupDto } from '@proxy/units';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { Observable, Subject } from 'rxjs';
import { finalize, takeUntil } from 'rxjs/operators';
import { DeleteUnitGroup, GetUnitGroups, SelectUnitGroup, UpdateRequestDto } from '../../actions';

import { FilteredPagedAndSortedResultRequestDto } from '@proxy';
import { DialogType, toPascalCase } from 'src/app/utils';
import { Condition } from '@allegorysoft/filter';

import { UNIT_GROUP_FILTER_STATE_TOKEN } from '../../tokens';

@Component({
  selector: 'app-unit-group-list',
  templateUrl: './unit-group-list.component.html',
  styleUrls: ['./unit-group-list.component.scss']
})
export class UnitGroupListComponent implements OnInit, OnDestroy {
  //#region Fields
  private destroy$: Subject<void> = new Subject<void>();
  @Select(UnitState.getUnitGroups) unitGroups$: Observable<UnitGroupDto[]>;

  @ViewChild('unitGroupDt') table: Table;

  @Select(state => state.unit!.totalCount) totalRecords: Observable<number>;
  @Select(state => state.app!.pageSizeOptions) pageSizeOptions$: Observable<number[]>;
  pageSize: number;
  loading: boolean = false;

  filterStateToken: string = UNIT_GROUP_FILTER_STATE_TOKEN;
  createDialogVisible: boolean = false;
  editDialogVisible: boolean = false;
  selectedUnitGroupId: number;
  //#endregion

  //#region Utilities
  private getOrder = (sortOrder: number): string => sortOrder === -1 ? 'desc' : 'asc';

  public get tableConfig(): any {
    return {
      dataKey: 'id',
      lazy: true,
      paginator: true,
      showCurrentPageReport: false,
      sortMode: 'multiple'
    };
  }
  //#endregion

  //#region Ctor
  constructor(
    private store: Store,
    private confirmation: ConfirmationService,
    private cdr: ChangeDetectorRef
  ) { }
  //#endregion

  //#region Methods
  ngOnInit(): void {
    this.pageSizeOptions$
      .pipe(takeUntil(this.destroy$))
      .subscribe(response => this.pageSize = response[0]);
  }

  loadUnitGroups(event: LazyLoadEvent): void {
    this.loading = true;
    this.pageSize = event.rows;
    this.cdr.detectChanges();

    const conditions: Condition = <Condition>{ group: [] };

    //#region Filtering
    Object.entries(event.filters).map(prop => {
      if (prop[1].value) {
        const condition: Condition = <Condition>{
          column: toPascalCase(prop[0]),
          operator: <any>toPascalCase(prop[1].matchMode),
          value: prop[1].value
        };
        conditions.group.push(condition)
      }
    });
    //#endregion

    //#region Sorting
    let sorting = '';

    if (event.multiSortMeta?.length > 0) {
      event.multiSortMeta.map((item, i) => {
        sorting +=
          `${item.field} ` +
          this.getOrder(item.order) +
          `${(i < event.multiSortMeta.length - 1) ? ', ' : ''}`;
      });
    }
    //#endregion

    const requestDto = <FilteredPagedAndSortedResultRequestDto>{
      maxResultCount: event.rows,
      conditions: conditions.group.length > 0 ? conditions : undefined,
      skipCount: (event.first ?? 0),
      sorting: sorting
    };

    this.store
      .dispatch(new GetUnitGroups(requestDto))
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => {
          this.loading = false;
          this.store.dispatch(new UpdateRequestDto(requestDto));
        })
      )
      .subscribe(() => { });
  }

  selectUnitGorup(unitGroup: UnitGroupDto): void {
    this.store.dispatch(new SelectUnitGroup(unitGroup));
  }

  showDialog = (dialogType: DialogType, unitGroupId: number = null): any => {
    switch (dialogType) {
      case DialogType.Create: this.createDialogVisible = true; break;
      case DialogType.Edit:
        this.selectedUnitGroupId = unitGroupId;
        this.editDialogVisible = true;
        break;
    }
  }

  deleteOnClick(id: number, name: string): void {
    const options = { messageLocalizationParams: [name] } as Partial<Confirmation.Options>;
    this.confirmation
      .warn('::UnitGroupDeletionConfirmationMessage', '::AreYouSure', options)
      .pipe(takeUntil(this.destroy$))
      .subscribe(response => {
        if (response === Confirmation.Status.confirm)
          this.store.dispatch(new DeleteUnitGroup(id));
      });
  }

  clearSort(): void {
    this.table.sortOrder = 0;
    this.table.sortField = '';
    this.table.reset();
    localStorage.removeItem(this.filterStateToken);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
  //#endregion
}
