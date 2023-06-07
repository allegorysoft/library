import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Select, Store } from '@ngxs/store';
import { DeductionDto } from '@proxy/calculations/product';
import { Observable } from 'rxjs';
import { GetDeductions } from '../../actions';
import { ProductCalculationState } from '../../states';

@Component({
  selector: 'app-deduction-list',
  templateUrl: './deduction-list.component.html',
  styleUrls: ['./deduction-list.component.scss']
})
export class DeductionListComponent implements OnInit {
  //#region Fields
  @Select(ProductCalculationState.getDeductions) deductions$!: Observable<DeductionDto[]>;
  @Select(ProductCalculationState.getRetrievingData) retrievingData$!: Observable<boolean>;

  @Output() deductionSelected: EventEmitter<DeductionDto> = new EventEmitter<DeductionDto>();
  //#endregion

  //#region Utilities
  get tableConfig(): any {
    return {
      dataKey: 'deductionCode',
      lazy: false,
      paginator: false,
      showCurrentPageReport: false,
      pageSize: 3,
      pageSizeOptions: [3, 5, 10]
    };
  }
  //#endregion

  //#region Ctor
  constructor(private store: Store) { }
  //#endregion

  //#region Methods
  ngOnInit(): void {
    this.store.dispatch(new GetDeductions());
  }

  selectDeduction = (deduction: DeductionDto): any => this.deductionSelected.emit(deduction)
  //#endregion
}
