import { CoreModule } from '@abp/ng.core';
import { NgModule } from '@angular/core';
import { PublicLayoutComponent } from './public-layout.component';

@NgModule({
    declarations: [PublicLayoutComponent],
    imports: [CoreModule],
    exports: [PublicLayoutComponent],
})
export class PublicLayoutModule { }
