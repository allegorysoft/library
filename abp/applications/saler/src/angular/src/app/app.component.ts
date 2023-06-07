import { Component, OnInit } from '@angular/core';
import { ReplaceableComponentsService, SessionStateService } from '@abp/ng.core';
import { eThemeBasicComponents } from '@abp/ng.theme.basic';
import { NavComponent } from './shared/components/nav/nav.component';
import { LogoComponent } from './shared/components/logo/logo.component';
import { NavItemsComponent } from './shared/components/nav-items/nav-items.component';
import { PrimeNGConfig } from 'primeng/api';
import { LayoutComponent } from './shared/components/layout/layout.component';

@Component({
  selector: 'app-root',
  template: `
    <abp-loader-bar></abp-loader-bar>
    <abp-dynamic-layout></abp-dynamic-layout>
  `,
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  //#region Fields
  private readonly defaultLang: string = 'tr';
  //#endregion

  //#region Ctor
  constructor(
    private replaceableComponents: ReplaceableComponentsService,
    private sessionState: SessionStateService,
    private primengConfig: PrimeNGConfig
  ) { }
  //#endregion

  //#region Methods
  ngOnInit(): void {
    this.primengConfig.ripple = true;
    this.sessionState.setLanguage(this.defaultLang);

    //Replace app layout
    this.replaceableComponents.add({
      component: LayoutComponent,
      key: eThemeBasicComponents.ApplicationLayout,
    });

    //Replace logo
    // this.replaceableComponents.add({
    //   component: LogoComponent,
    //   key: eThemeBasicComponents.Logo,
    // });

    // //Replace nav-items
    // this.replaceableComponents.add({
    //   component: NavItemsComponent,
    //   key: eThemeBasicComponents.NavItems,
    // });
  }
  //#endregion
}
