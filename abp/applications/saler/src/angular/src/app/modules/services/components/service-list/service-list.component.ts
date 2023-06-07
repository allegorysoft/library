import {
  Component,
  OnInit,
  ChangeDetectorRef,
  EventEmitter,
  Inject,
  Output,
  ViewChild,
  OnDestroy
} from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { finalize, takeUntil } from 'rxjs/operators';
import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { Select, Store } from '@ngxs/store';
import { Condition } from '@allegorysoft/filter';
import { FilteredPagedAndSortedResultRequestDto } from '@proxy';
import { ServiceDto } from '@proxy/services';
import { OBSERVE, Observed, ObserveFn, OBSERVE_PROVIDER } from 'ng-observe';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { DialogType, toPascalCase } from 'src/app/utils';
import { DeleteService, GetServices, UpdateRequestDto } from '../../actions';
import { ServiceState } from '../../states';
import { SERVICE_FILTER_STATE_TOKEN } from '../../tokens';

@Component({
  selector: 'app-service-list',
  templateUrl: './service-list.component.html',
  providers: [OBSERVE_PROVIDER]
})
export class ServiceListComponent implements OnInit, OnDestroy {
  //#region Fields
  private destroy$: Subject<void> = new Subject<void>();

  @ViewChild('serviceDt') table: Table;
  @Select(ServiceState.getServices) services$: Observable<ServiceDto[]>;
  @Select(ServiceState.getLoading) loading$: Observable<boolean>;
  @Select(ServiceState.getTotalCount) totalRecords$: Observable<number>;

  @Output() selectServiceChanged = new EventEmitter<ServiceDto>();

  pageSizeOptions: Observed<number[]>;
  filterStateToken: string = SERVICE_FILTER_STATE_TOKEN;
  pageSize: number;
  createDialogVisible: boolean = false;
  editDialogVisible: boolean = false;
  selectedServiceId: number;
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
    this.pageSize = this.pageSizeOptions.value[0];
  }

  loadServices(event: LazyLoadEvent): void {
    this.pageSize = event.rows;

    const conditions: Condition = <Condition>{ group: [] };

    //#region Filtering
    Object.entries(event.filters).map(prop => {
      if (prop[1].value || prop[1].value === 0) {
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
      .dispatch(new GetServices(requestDto))
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.store.dispatch(new UpdateRequestDto(requestDto)))
      )
      .subscribe(() => { });
    this.cdr.detectChanges();
  }

  selectService(service: ServiceDto): void {
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
    this.destroy$.next();
    this.destroy$.complete();
  }
  //#endregion
}
