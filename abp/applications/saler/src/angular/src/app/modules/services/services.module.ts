import { NgModule } from '@angular/core';
import { NgxsModule } from '@ngxs/store';

import { ServicesRoutingModule } from './services-routing.module';

import { ServiceState } from './states';

import { ServiceListComponentModule } from './components/service-list/service-list-component.module';

import { ServicesComponent } from './services.component';


@NgModule({
  declarations: [ServicesComponent],
  imports: [
    ServicesRoutingModule,
    NgxsModule.forFeature([
      ServiceState
    ]),
    ServiceListComponentModule
  ],
  exports: [ServicesComponent]
})
export class ServicesModule { }
