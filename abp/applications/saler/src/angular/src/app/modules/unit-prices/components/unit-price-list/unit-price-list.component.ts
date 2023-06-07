import {
  Component,
  ChangeDetectorRef,
  Inject,
  Input,
  OnDestroy,
  OnInit,
  ViewChild
} from '@angular/core';
import { Select, Store } from '@ngxs/store';
import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { FilteredPagedAndSortedResultRequestDto } from '@proxy';
import { UnitPriceDto, UnitPriceType } from '@proxy/unit-prices';
import { OBSERVE, Observed, ObserveFn, OBSERVE_PROVIDER } from 'ng-observe';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { Condition, Operator } from '@allegorysoft/filter';
import { toPascalCase } from 'src/app/utils';
import { DeleteUnitPrice, GetUnitPrice, GetUnitPrices } from '../../actions';
import { UnitPriceState } from '../../states';
import { UNIT_PRICE_FILTER_STATE_TOKEN } from '../../tokens';

@Component({
  selector: 'app-unit-price-list',
  templateUrl: './unit-price-list.component.html',
  providers: [OBSERVE_PROVIDER]
})
export class UnitPriceListComponent implements OnInit, OnDestroy {
  //#region Fields
  private destroy$: Subject<void> = new Subject<void>();
  @Input() type!: UnitPriceType;

  @ViewChild('unitPriceDt') table!: Table;
  @Select(UnitPriceState.getUnitPrices) unitPrices$: Observable<UnitPriceDto[]>;
  @Select(UnitPriceState.getTotalCount) totalRecords$: Observable<number>;
  @Select(UnitPriceState.getLoading) loading$: Observable<boolean>;

  pageSizeOptions: Observed<number[]>;
  filterStateToken: string = UNIT_PRICE_FILTER_STATE_TOKEN;
  pageSize!: number;
  columnCount: number = 6;
  isDialogOpen: boolean = false;
  //#endregion

  //#region Utilities
  private getOrder = (sortOrder: number): string => sortOrder === -1 ? 'desc' : 'asc';

  get tableConfig(): any {
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
    private cdr: ChangeDetectorRef,
    private readonly confirmation: ConfirmationService
  ) {
    this.pageSizeOptions = this.observe(this.store.select(state => state!.app!.pageSizeOptions));
  }
  //#endregion

  //#region Methods
  ngOnInit(): void {
    this.pageSize = this.pageSizeOptions.value[0];
  }

  createUnitPrice(): void {
    this.isDialogOpen = true;
  }

  editUnitPrice(id: number): void {
    this.store.dispatch(new GetUnitPrice(id));
    this.isDialogOpen = true;
  }

  loadUnitPrices(event: LazyLoadEvent): void {
    this.pageSize = event.rows;

    const conditions = <Condition>{
      group: [{
        column: 'Type',
        operator: Operator.Equals,
        value: this.type,
      }]
    };

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

        const condition: Condition = <Condition>{
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

    const requestDto = <FilteredPagedAndSortedResultRequestDto>{
      maxResultCount: event.rows,
      conditions: conditions,
      skipCount: (event.first ?? 0),
      sorting: sorting
    };

    this.store.dispatch(new GetUnitPrices(requestDto));
    this.cdr.detectChanges();
  }

  delete(id: number, name: string): void {
    const options = { messageLocalizationParams: [name] } as Partial<Confirmation.Options>;
    this.confirmation
      .warn('::UnitPriceDeletionConfirmationMessage', '::AreYouSure', options)
      .pipe(takeUntil(this.destroy$))
      .subscribe(response => {
        if (response === Confirmation.Status.confirm)
          this.store.dispatch(new DeleteUnitPrice(id));
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
