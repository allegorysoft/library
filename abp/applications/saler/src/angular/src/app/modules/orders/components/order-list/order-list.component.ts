import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { Component, OnInit, OnDestroy, ChangeDetectorRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Select, Store } from '@ngxs/store';
import { FilteredPagedAndSortedResultRequestDto } from '@proxy';
import { OrderDto, OrderStatu, OrderType } from '@proxy/orders';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { Observable, Subject } from 'rxjs';
import { finalize, takeUntil } from 'rxjs/operators';
import { Condition, Operator } from '@allegorysoft/filter';
import { DeleteOrder, GetOrders, UpdateRequestDto } from '../../actions';
import { OrderState } from '../../states';
import { orderStatuOptions } from 'src/app/proxy/orders'
import { ABP, ConfigStateService } from '@abp/ng.core';
import { toPascalCase } from 'src/app/utils';
import { ORDER_FILTER_STATE_TOKEN } from '../../tokens';

@Component({
  selector: 'app-order-list',
  templateUrl: './order-list.component.html'
})
export class OrderListComponent implements OnInit, OnDestroy {
  //#region Fields
  private _lastEvent: LazyLoadEvent;
  private destroy$: Subject<void> = new Subject<void>();

  filterStateToken: string = ORDER_FILTER_STATE_TOKEN;
  orderType: OrderType;
  @Select(OrderState.getOrders) orders$: Observable<OrderDto[]>;
  @ViewChild('orderDt') table: Table;

  @Select(state => state.order!.totalCount) totalRecords$: Observable<number>;
  @Select(state => state.app!.pageSizeOptions) pageSizeOptions$: Observable<number[]>;
  pageSize: number;
  loading: boolean = false;
  navigateToEdit: boolean = false;

  orderStatuOptions: ABP.Option<typeof OrderStatu>[] = orderStatuOptions;
  datePattern: string = 'MM/d/yyyy';
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

  get urlByOrderType(): string {
    return this.orderType === 0 ? 'purchase' : 'sales'
  }
  //#endregion

  //#region Ctor
  constructor(
    private store: Store,
    private cdr: ChangeDetectorRef,
    private activatedRoute: ActivatedRoute,
    private confirmation: ConfirmationService,
    private router: Router,
    private config: ConfigStateService
  ) { }
  //#endregion

  //#region Methods
  ngOnInit(): void {
    this.activatedRoute
      .paramMap
      .pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        if (params) {
          const orderType = params.get('type');
          this.orderType = <OrderType>(orderType === 'purchase' ? 0 : orderType === 'sales' ? 1 : 0);
          if (this._lastEvent) {
            this.table.first = 0;
            this.loadOrders({ ...this._lastEvent, first: 0 });
          }
        }
      });

    this.pageSizeOptions$
      .pipe(takeUntil(this.destroy$))
      .subscribe(response => this.pageSize = response[0]);

    this.config.getDeep$('localization')
      .pipe(takeUntil(this.destroy$))
      .subscribe(response =>
        this.datePattern = response.currentCulture.dateTimeFormat.shortDatePattern
      );
  }

  loadOrders(event: LazyLoadEvent): void {
    this.loading = true;
    this.pageSize = event.rows;

    const conditions = <Condition>{
      group: [{
        column: 'Type',
        operator: Operator.Equals,
        value: this.orderType,
      }]
    };

    //#region Filtering
    Object.entries(event.filters).map(prop => {
      if (prop[1].value || prop[1].value === 0) {
        //#region refactor here
        let matchMode = prop[1].matchMode;
        let not: boolean = false;
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

    const requestDto = <FilteredPagedAndSortedResultRequestDto>{
      maxResultCount: event.rows,
      conditions: conditions,
      skipCount: (event.first ?? 0),
      sorting: sorting
    };

    this.store
      .dispatch(new GetOrders(requestDto))
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => {
          this.loading = false;
          this.store.dispatch(new UpdateRequestDto(requestDto));
          this._lastEvent = event;
        })
      )
      .subscribe(() => { });
    this.cdr.detectChanges();
  }

  edit(id: number): void {
    this.navigateToEdit = true;
    this.router.navigate(['/orders/' + this.urlByOrderType + '/edit/' + id]);
  }

  delete(id: number, orderNumber: string): void {
    const options = { messageLocalizationParams: [orderNumber] } as Partial<Confirmation.Options>;
    this.confirmation
      .warn('::OrderDeletionConfirmationMessage', '::AreYouSure', options)
      .pipe(takeUntil(this.destroy$))
      .subscribe(response => {
        if (response === Confirmation.Status.confirm)
          this.store.dispatch(new DeleteOrder(id));
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
