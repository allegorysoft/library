import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { Select, Store } from '@ngxs/store';
import { GlobalUnitDto } from '@proxy/units';
import { Observable } from 'rxjs';
import { GetGlobalUnits } from '../../actions';
import { UnitState } from '../../states';

@Component({
  selector: 'app-global-unit-list',
  templateUrl: './global-unit-list.component.html',
  styleUrls: ['./global-unit-list.component.scss']
})
export class GlobalUnitListComponent implements OnInit {
  //#region Fields
  @Select(UnitState.getGlobalUnits) globalUnits$: Observable<GlobalUnitDto[]>;
  filterStateToken: string = 'global_unit_filter';
  @Output() globalUnitSelected: EventEmitter<GlobalUnitDto> = new EventEmitter<GlobalUnitDto>();
  //#endregion

  //#region Utilities
  get tableConfig(): any {
    return {
      dataKey: 'code'
    };
  }
  //#endregion

  //#region Ctor
  constructor(private store: Store) { }
  //#endregion

  //#region Methods
  ngOnInit(): void {
    this.store.dispatch(new GetGlobalUnits());
  }

  selectGlobalUnit(globalUnit: GlobalUnitDto): void {
    this.globalUnitSelected.emit(globalUnit);
  }
  //#endregion
}
