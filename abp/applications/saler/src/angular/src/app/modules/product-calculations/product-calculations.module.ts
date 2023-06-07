import { NgModule } from '@angular/core';
import { NgxsModule } from '@ngxs/store';
import { SharedModule } from 'src/app/shared/shared.module';

import { ProductCalculationState } from './states';

import { DeductionListComponent } from './components/deduction-list/deduction-list.component';

const declarations = [DeductionListComponent];

@NgModule({
  declarations: [...declarations],
  imports: [
    SharedModule,
    NgxsModule.forFeature([
      ProductCalculationState
    ]),
  ],
  exports: [...declarations]
})
export class ProductCalculationsModule { }
