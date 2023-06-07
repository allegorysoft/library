import { Component } from '@angular/core';
import { Select, Store } from '@ngxs/store';
import { Observable } from 'rxjs';
import { ToggleSidebar } from '../../actions';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html'
})
export class SidebarComponent {
  @Select(state => state.app.sidebarVisible) visible$: Observable<boolean>;

  constructor(private store: Store) { }

  visibleChange(event: boolean): void {
    this.store.dispatch(new ToggleSidebar(event));
  }
}
