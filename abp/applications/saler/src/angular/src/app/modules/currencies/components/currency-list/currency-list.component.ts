import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Select, Store } from '@ngxs/store';
import { FilteredPagedAndSortedResultRequestDto } from '@proxy';
import { CurrencyDto } from '@proxy/currencies';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { Observable, Subject } from 'rxjs';
import { finalize, takeUntil } from 'rxjs/operators';
import { Condition } from '@allegorysoft/filter';
import { toPascalCase } from 'src/app/utils';
import { GetCurrencies, GetCurrency, UpdateRequestDto, DeleteCurrency } from '../../actions';
import { CurrencyState } from '../../states';
import { CURRENCY_FILTER_STATE_TOKEN } from '../../tokens';

@Component({
  selector: 'app-currency-list',
  templateUrl: './currency-list.component.html',
  styleUrls: ['./currency-list.component.scss']
})
export class CurrencyListComponent implements OnInit, OnDestroy {
  //#region Fields
  filterStateToken: string = CURRENCY_FILTER_STATE_TOKEN;
  destroy$: Subject<void> = new Subject<void>();
  @Select(CurrencyState.getCurrencies) currencies$: Observable<CurrencyDto[]>;

  @ViewChild('currencyDt') table: Table;

  @Select(CurrencyState.getTotalCount) totalCount: Observable<number>;
  @Select(state => state.app!.pageSizeOptions) pageSizeOptions$: Observable<number[]>;
  pageSize: number;
  loading: boolean = false;
  dialogVisible: boolean = false;
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

  loadCurrencies(event: LazyLoadEvent): void {
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
      conditions: conditions.group.length > 0 ? conditions : undefined,
      skipCount: (event.first ?? 0),
      sorting: sorting
    };

    this.store
      .dispatch(new GetCurrencies(requestDto))
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => {
          this.loading = false;
          this.store.dispatch(new UpdateRequestDto(requestDto));
        })
      )
      .subscribe(() => { });
  }

  selectCurrency(currency: CurrencyDto): void {
    console.log(currency);
  }

  showDialog(currency: CurrencyDto): void {
    if (currency && currency.id > 0)
      this.store.dispatch(new GetCurrency(currency.id))
        .pipe(takeUntil(this.destroy$))
        .subscribe(() => this.dialogVisible = true);
    else
      this.dialogVisible = true;
  }

  deleteOnClick(id: number, name: string): void {
    const options = { messageLocalizationParams: [name] } as Partial<Confirmation.Options>;
    this.confirmation
      .warn('::CurrencyDeletionConfirmationMessage', '::AreYouSure', options)
      .pipe(takeUntil(this.destroy$))
      .subscribe(response => {
        if (response === Confirmation.Status.confirm)
          this.store.dispatch(new DeleteCurrency(id));
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
