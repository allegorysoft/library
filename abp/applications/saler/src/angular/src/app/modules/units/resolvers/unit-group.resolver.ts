import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { Store } from '@ngxs/store';
import { UnitGroupDto } from '@proxy/units';
import { Observable, of } from 'rxjs';
import { GetUnitGroups } from '../actions';

@Injectable({
  providedIn: 'root'
})
export class UnitGroupResolver implements Resolve<UnitGroupDto> {

  constructor(private store: Store) { }

  resolve(route: ActivatedRouteSnapshot): Observable<UnitGroupDto> {
    const maxResultCount = this.store.selectSnapshot(state => state.unit.requestDto.maxResultCount) as number;
    const skipCount = this.store.selectSnapshot(state => state.unit.requestDto.skipCount) as number;

    return this.store.dispatch(new GetUnitGroups({
      maxResultCount: maxResultCount,
      skipCount: skipCount,
      conditions: null,
      sorting: null
    }));
  }
}
