import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router, ParamMap } from '@angular/router';
import { Store } from '@ngxs/store';
import { OrderWithDetailsDto } from '@proxy/orders';
import { Observable } from 'rxjs';
import { GetOrder } from '../actions';

@Injectable({
  providedIn: 'root'
})
export class OrderEditResolver implements Resolve<OrderWithDetailsDto> {
  constructor(
    private store: Store,
    private router: Router
  ) { }

  resolve(route: ActivatedRouteSnapshot): Observable<OrderWithDetailsDto> {
    const paramMap: ParamMap = route.paramMap;
    const id: number = +paramMap.get('id');

    if (id < 1) this.router.navigate(['/']);

    return this.store.dispatch(new GetOrder(id));
  }
}
