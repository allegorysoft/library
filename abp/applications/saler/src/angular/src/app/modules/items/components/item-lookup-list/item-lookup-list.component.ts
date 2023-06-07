import {
  Component,
  ChangeDetectorRef,
  EventEmitter,
  Inject,
  OnInit,
  Input,
  Output,
  ViewChild,
  OnDestroy
} from '@angular/core';
import { ABP, isNullOrUndefined } from '@abp/ng.core';
import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { Select, Store } from '@ngxs/store';
import { GetItemLookupListDto, ItemLookupDto, ItemType, itemTypeOptions } from '@proxy/items';
import { OBSERVE, Observed, ObserveFn, OBSERVE_PROVIDER } from 'ng-observe';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { Observable, Subject } from 'rxjs';
import { finalize, takeUntil } from 'rxjs/operators';
import { Condition } from '@allegorysoft/filter';
import { DialogType, toPascalCase } from 'src/app/utils';
import {
  PatchWorkOnLookup,
  GetLookupItems,
  UpdateLookupRequestDto,
  DeleteItem,
} from '../../actions';
import { ItemState } from '../../states';
import { ITEM_LOOKUP_FILTER_STATE_TOKEN } from '../../tokens';

@Component({
  selector: 'app-item-lookup-list',
  templateUrl: './item-lookup-list.component.html',
  providers: [OBSERVE_PROVIDER]
})
export class ItemLookupListComponent implements OnInit, OnDestroy {
  //#region Fields
  private destroy$: Subject<void> = new Subject<void>();

  @Input() date: string;
  @Input() isSales: boolean;
  @Input() clientCode?: string;

  @ViewChild('itemLookupDt') table: Table;
  @Select(ItemState.getLookupItems) items$: Observable<ItemLookupDto[]>;
  @Select(ItemState.getLoadingLookup) loading$: Observable<boolean>;
  @Select(ItemState.getLookupTotalCount) totalRecords$: Observable<number>;

  @Output() selectItemChanged = new EventEmitter<ItemLookupDto>();

  pageSizeOptions: Observed<number[]>;
  filterStateToken: string = ITEM_LOOKUP_FILTER_STATE_TOKEN;
  pageSize: number;
  itemTypeOptions: ABP.Option<typeof ItemType>[] = itemTypeOptions;
  createDialogVisible: boolean = false;
  editDialogVisible: boolean = false;
  selectedId: number;
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
    @Inject(OBSERVE) private observe: ObserveFn,
    private store: Store,
    private confirmation: ConfirmationService,
    private cdr: ChangeDetectorRef
  ) {
    this.pageSizeOptions = this.observe(this.store.select(state => state!.app!.pageSizeOptions));
  }
  //#endregion

  //#region Methods
  ngOnInit(): void {
    this.store.dispatch(new PatchWorkOnLookup(true));
    this.pageSize = this.pageSizeOptions.value[0];
  }

  loadItems(event: LazyLoadEvent): void {
    this.pageSize = event.rows;
    this.cdr.detectChanges();

    const conditions = <Condition>{ group: [] };

    //#region Filtering
    Object.entries(event.filters).map(prop => {
      if (prop[1].value || prop[1].value === 0) {
        //#region refactor here
        let matchMode = prop[1].matchMode;
        let not = false;
        switch (matchMode) {
          case 'notEquals': matchMode = 'DoesntEquals'; break;
          case 'lt': matchMode = 'IsLessThan'; break;
          case 'lte': matchMode = 'IsLessThanOrEqualto'; break;
          case 'gt': matchMode = 'IsGreaterThan'; break;
          case 'gte': matchMode = 'IsGreaterThanOrEqualto'; break;
          case 'notContains': matchMode = 'Contains'; not = true; break;
        }
        //#endregion

        const condition = <Condition>{
          column: toPascalCase(prop[0]),
          operator: <any>toPascalCase(matchMode),
          value: prop[1].value,
          not: not
        };
        conditions.group.push(condition);
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

    const requestDto = <GetItemLookupListDto>{
      maxResultCount: event.rows,
      conditions: conditions.group?.length > 0 ? conditions : undefined,
      skipCount: (event.first ?? 0),
      sorting: sorting,
      date: this.date,
      isSales: this.isSales,
      clientCode: this.clientCode
    };

    this.store
      .dispatch(new GetLookupItems(requestDto))
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.store.dispatch(new UpdateLookupRequestDto(requestDto)))
      )
      .subscribe(() => { });
    this.cdr.detectChanges();
  }

  selectItem(item: ItemLookupDto): void {
    if (item) this.selectItemChanged.emit(item);
  }

  showDialog = (dialogType: DialogType, id?: number): any => {
    switch (dialogType) {
      case DialogType.Create: this.createDialogVisible = true; break;
      case DialogType.Edit:
        if (!isNullOrUndefined(id))
          this.selectedId = id;
        this.editDialogVisible = true;
        break;
    }
  }

  deleteOnClick(id: number, code: string): void {
    const options = { messageLocalizationParams: [code] } as Partial<Confirmation.Options>;
    this.confirmation
      .warn('::ItemDeletionConfirmationMessage', '::AreYouSure', options)
      .pipe(takeUntil(this.destroy$))
      .subscribe(response => {
        if (response === Confirmation.Status.confirm) {
          this.store.dispatch(new DeleteItem(id));
        }
      });
  }

  clearSort(): void {
    this.table.sortOrder = 0;
    this.table.sortField = '';
    this.table.reset();
    localStorage.removeItem(this.filterStateToken);
  }

  ngOnDestroy(): void {
    this.store.dispatch(new PatchWorkOnLookup(false));
    this.destroy$.next();
    this.destroy$.complete();
  }
  //#endregion
}
