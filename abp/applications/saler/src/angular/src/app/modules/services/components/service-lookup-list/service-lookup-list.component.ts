import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { ChangeDetectorRef, Component, EventEmitter, Inject, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Select, Store } from '@ngxs/store';
import { GetServiceLookupListDto, ServiceLookupDto } from '@proxy/services';
import { OBSERVE, Observed, ObserveFn, OBSERVE_PROVIDER } from 'ng-observe';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { Observable, Subject } from 'rxjs';
import { finalize, takeUntil } from 'rxjs/operators';
import { Condition } from '@allegorysoft/filter';
import { DialogType, getCondition, getOrder } from 'src/app/utils';
import { DeleteService, GetLookupServices, PatchWorkOnLookup, UpdateLookupRequestDto } from '../../actions';
import { ServiceState } from '../../states';
import { SERVICE_LOOKUP_FILTER_STATE_TOKEN } from '../../tokens';

@Component({
  selector: 'app-service-lookup-list',
  templateUrl: './service-lookup-list.component.html',
  providers: [OBSERVE_PROVIDER]
})
export class ServiceLookupListComponent implements OnInit {
  //#region Fields
  private destroy$: Subject<void> = new Subject<void>();

  @Input() date: string;
  @Input() isSales: boolean;
  @Input() clientCode?: string;

  @ViewChild('serviceLookupDt') table: Table;
  @Select(ServiceState.getLookupServices) services$: Observable<ServiceLookupDto[]>;
  @Select(ServiceState.getLoadingLookup) loading$: Observable<boolean>;
  @Select(ServiceState.getLookupTotalCount) totalRecords$: Observable<number>;

  @Output() selectServiceChanged = new EventEmitter<ServiceLookupDto>();

  pageSizeOptions: Observed<number[]>;
  filterStateToken: string = SERVICE_LOOKUP_FILTER_STATE_TOKEN;
  pageSize: number;
  createDialogVisible: boolean = false;
  editDialogVisible: boolean = false;
  selectedServiceId: number;
  //#endregion

  //#region Utilities
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

  loadServices(event: LazyLoadEvent): void {
    this.pageSize = event.rows;

    const conditions: Condition = <Condition>{ group: [] };

    //#region Filtering
    Object.entries(event.filters).map(prop => {
      if (prop[1].value || prop[1].value === 0) {
        const condition = getCondition(
          prop[0],
          prop[1].matchMode,
          prop[1].value
        );
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
          getOrder(item.order) +
          `${(i < event.multiSortMeta.length - 1) ? ', ' : ''}`;
      });
    }
    //#endregion

    const requestDto = <GetServiceLookupListDto>{
      maxResultCount: event.rows,
      conditions: conditions.group.length > 0 ? conditions : undefined,
      skipCount: (event.first ?? 0),
      sorting: sorting,
      date: this.date,
      isSales: this.isSales,
      clientCode: this.clientCode
    };

    this.store
      .dispatch(new GetLookupServices(requestDto))
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.store.dispatch(new UpdateLookupRequestDto(requestDto)))
      )
      .subscribe(() => { });
    this.cdr.detectChanges();
  }

  selectService(service: ServiceLookupDto): void {
    if (service) {
      this.selectServiceChanged.emit(service);
    }
  }

  deleteOnClick(id: number, code: string): void {
    const options = { messageLocalizationParams: [code] } as Partial<Confirmation.Options>;
    this.confirmation
      .warn('::ServiceDeletionConfirmationMessage', '::AreYouSure', options)
      .pipe(takeUntil(this.destroy$))
      .subscribe(response => {
        if (response === Confirmation.Status.confirm) {
          this.store.dispatch(new DeleteService(id));
        }
      });
  }

  showDialog = (dialogType: DialogType, serviceId: number = null): any => {
    switch (dialogType) {
      case DialogType.Create: this.createDialogVisible = true; break;
      case DialogType.Edit:
        this.selectedServiceId = serviceId;
        this.editDialogVisible = true;
        break;
    }
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
