import { CoreModule } from '@abp/ng.core';
import { NgModule } from '@angular/core';
import { AdminLayoutComponent } from './admin-layout.component';

@NgModule({
    declarations: [AdminLayoutComponent],
    imports: [CoreModule],
    exports: [AdminLayoutComponent],
})
export class AdminLayoutModule { }
