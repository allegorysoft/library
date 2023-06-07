import { CoreModule } from '@abp/ng.core';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { NgModule } from '@angular/core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgxValidateCoreModule } from '@ngx-validate/core';

//#region primeng
import * as Prime from 'primeng/api';
import { MenubarModule } from 'primeng/menubar';
import { ButtonModule } from 'primeng/button';
import { SidebarModule } from 'primeng/sidebar';
import { SlideMenuModule } from 'primeng/slidemenu';
import { MenuModule } from 'primeng/menu';
import { TooltipModule } from 'primeng/tooltip';
import { TableModule } from 'primeng/table';
import { RippleModule } from 'primeng/ripple';
import { SpeedDialModule } from 'primeng/speeddial';
import { ToolbarModule } from 'primeng/toolbar';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputSwitchModule } from 'primeng/inputswitch';
import { CheckboxModule } from 'primeng/checkbox';
import { InputNumberModule } from 'primeng/inputnumber';
import { DropdownModule } from 'primeng/dropdown';
import { CardModule } from 'primeng/card';
import { CalendarModule } from 'primeng/calendar';
//#endregion

import { NavComponentModule } from './components/nav/nav-component.module';

import { LogoComponent } from './components/logo/logo.component';
import { NavItemsComponent } from './components/nav-items/nav-items.component';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { LayoutComponent } from './components/layout/layout.component';


const primeModules = [
  Prime.SharedModule,
  MenubarModule,
  SidebarModule,
  ButtonModule,
  SlideMenuModule,
  MenuModule,
  TableModule,
  SpeedDialModule,
  RippleModule,
  ToolbarModule,
  DynamicDialogModule,
  DialogModule,
  InputTextModule,
  InputSwitchModule,
  CheckboxModule,
  InputNumberModule,
  TooltipModule,
  DropdownModule,
  CardModule,
  CalendarModule
];

const declarations = [
  LogoComponent,
  NavItemsComponent,
  SidebarComponent,
  LayoutComponent
];

@NgModule({
  declarations: [...declarations],
  imports: [
    CoreModule,
    ThemeSharedModule,
    NgbDropdownModule,
    NgxValidateCoreModule,
    ...primeModules,
    NavComponentModule
  ],
  exports: [
    CoreModule,
    ThemeSharedModule,
    NgbDropdownModule,
    NgxValidateCoreModule,
    ...primeModules,
    ...declarations
  ]
})
export class SharedModule { }
