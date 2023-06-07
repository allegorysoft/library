import { NgModule } from '@angular/core';
import { NgxsModule } from '@ngxs/store';

import { ItemsRoutingModule } from './items-routing.module';

import { ItemState } from './states';

import { ItemListComponentModule } from './components/item-list/item-list-component.module';

import { ItemsComponent } from './items.component';


@NgModule({
  declarations: [ItemsComponent],
  imports: [
    ItemsRoutingModule,
    NgxsModule.forFeature([
      ItemState
    ]),
    ItemListComponentModule
  ],
  exports: [ItemsComponent],
})
export class ItemsModule { }
