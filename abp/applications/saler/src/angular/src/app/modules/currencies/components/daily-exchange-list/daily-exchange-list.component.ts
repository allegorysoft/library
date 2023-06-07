import { ToasterService } from '@abp/ng.theme.shared';
import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { Select, Store } from '@ngxs/store';
import { CurrencyDailyExchangeCreateUpdateDto, CurrencyDailyExchangeDto, CurrencyService } from '@proxy/currencies';
import * as moment from 'moment';
import { Table } from 'primeng/table';
import { Observable, Subject } from 'rxjs';
import { finalize, takeUntil } from 'rxjs/operators';
import { EditCurrencyDailyExchange, RefreshDailyExchanges } from '../../actions';
import { CurrencyState } from '../../states';
import { DAILY_EXCHANGE_FILTER_STATE_TOKEN } from '../../tokens';

@Component({
  selector: 'app-daily-exchange-list',
  templateUrl: './daily-exchange-list.component.html',
  styleUrls: ['./daily-exchange-list.component.scss']
})
export class DailyExchangeListComponent implements OnInit, OnDestroy {
  //#region Fields
  private destroy$: Subject<void> = new Subject<void>();
  @ViewChild('dailyExchangeDt') table: Table;

  @Select(state => state.app!.pageSizeOptions) pageSizeOptions$: Observable<number[]>;
  @Select(CurrencyState.getRefresingDailyExchanges)
  refresingDailyExchanges$: Observable<boolean>;

  dailyExchanges: CurrencyDailyExchangeDto[] = [];
  loading: boolean = false;
  filterStateToken: string = DAILY_EXCHANGE_FILTER_STATE_TOKEN;
  pageSize: number;
  date: Date = new Date(moment().format('YYYY-MM-DD'));
  clonedExchange: CurrencyDailyExchangeDto = undefined;
  //#endregion

  //#region Utilities
  private loadPageSizes(): void {
    this.pageSizeOptions$
      .pipe(takeUntil(this.destroy$))
      .subscribe(response => this.pageSize = response[0]);
  }

  get tableConfig(): any {
    return {
      dataKey: 'currencyCode',
      paginator: true,
      showCurrentPageReport: false,
    };
  }
  //#endregion

  //#region Ctor
  constructor(
    private readonly store: Store,
    private readonly currencyService: CurrencyService,
    private readonly toasterService: ToasterService
  ) { }
  //#endregion

  //#region Methods
  ngOnInit(): void {
    this.loadPageSizes();
    this.loadDailyExchanges();
  }

  loadDailyExchanges(): void {
    this.loading = true;
    this.dailyExchanges = [];

    this.currencyService
      .getCurrencyDailyExchangeList(moment(this.date).format('YYYY-MM-DD'))
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.loading = false)
      )
      .subscribe(
        response => this.dailyExchanges = response,
        err => this.toasterService.error(err, '::Error')
      );
  }

  refreshDailyExchange(): void {
    this.store.dispatch(new RefreshDailyExchanges())
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.loadDailyExchanges());
  }

  selectExchange(dailyExchange: CurrencyDailyExchangeDto): void {
    console.log(dailyExchange);
  }

  onRowEditInit(dailyExchange: CurrencyDailyExchangeDto): void {
    this.clonedExchange = Object.assign({}, dailyExchange);
  }

  onRowEditSave(dailyExchange: CurrencyDailyExchangeDto): void {
    const input: CurrencyDailyExchangeCreateUpdateDto = <CurrencyDailyExchangeCreateUpdateDto>{
      date: moment(this.date).format('YYYY-MM-DD'),
      currencyCode: dailyExchange.currencyCode,
      rate1: dailyExchange.rate1,
      rate2: dailyExchange.rate2,
      rate3: dailyExchange.rate3,
      rate4: dailyExchange.rate4,
    };
    this.store.dispatch(new EditCurrencyDailyExchange(input));
  }

  onRowEditCancel(index: number): void {
    this.dailyExchanges[index] = this.clonedExchange;
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
