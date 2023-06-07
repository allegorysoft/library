import { Component, OnInit, OnDestroy } from '@angular/core';
import {
  ABP,
  AuthService,
  ConfigStateService,
  CurrentUserDto,
  isUndefinedOrEmptyString,
  LocalizationService,
  RoutesService,
  TreeNode
} from '@abp/ng.core';
import { MenuItem } from 'primeng/api';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html'
})
export class NavComponent implements OnInit, OnDestroy {
  //#region Fields
  destroy$: Subject<void> = new Subject<void>();
  menuItems: MenuItem[] = [];
  accountItems: MenuItem[] = [];
  currentUser$: Observable<CurrentUserDto> = this.config.getOne$("currentUser") as Observable<CurrentUserDto>;
  //#endregion

  //#region Utilities
  private hasPath = (path: string): boolean => !isUndefinedOrEmptyString(path);

  private loadAccountItems(): void {
    [
      {
        label: 'AbpAccount::MyAccount',
        icon: 'pi pi-fw pi-user-edit',
        routerLink: ['/account/manage']
      },
      {
        label: 'AbpUi::Logout',
        icon: 'pi pi-fw pi-power-off',
        command: (event) => this.logOut()
      }
    ].map(item => {
      this.localizationService
        .get(item.label)
        .pipe(takeUntil(this.destroy$))
        .subscribe(localizedName => {
          item.label = localizedName;
          this.accountItems.push(item);
        });
    });
  }

  private loadMenuItems(): void {
    this.routesService.visible$
      .pipe(takeUntil(this.destroy$))
      .subscribe(visible => {
        this.menuItems = [];
        visible.map(root => {
          this.localizationService
            .get(root.name)
            .pipe(takeUntil(this.destroy$))
            .subscribe(localizedName => {
              let menuItem = {
                label: localizedName,
                icon: root.iconClass,
                visible: (this.hasPath(root.path) || root.children.length > 0),
                routerLink: root.path
              } as MenuItem;

              if (!root.isLeaf)
                this.configureChilds(root, menuItem);

              this.menuItems.push(menuItem);
            })
        })
      });
  }

  private configureChilds(treeRoot: TreeNode<ABP.Route>, menuItem: MenuItem): void {
    menuItem.items = [];
    for (const child of treeRoot.children) {
      this.localizationService
        .get(child.name)
        .pipe(takeUntil(this.destroy$))
        .subscribe(localizedName => {
          let childItem = {
            label: localizedName,
            icon: child.iconClass,
            visible: (this.hasPath(child.path) || child.children.length > 0),
            routerLink: child.path
          } as MenuItem;

          if (!child.isLeaf)
            this.configureChilds(child, childItem);

          menuItem.items.push(childItem);
        });
    }
  }

  get isAuthenticated(): boolean {
    return this.config.getDeep('currentUser.isAuthenticated') as boolean;
  }
  //#endregion

  //#region Ctor
  constructor(
    private readonly config: ConfigStateService,
    private readonly authService: AuthService,
    private readonly localizationService: LocalizationService,
    private readonly routesService: RoutesService
  ) { }
  //#endregion

  //#region Methods
  ngOnInit() {
    this.loadAccountItems();

    if (this.isAuthenticated)
      this.loadMenuItems();
  }

  navigateToLogin = (): void => this.authService.navigateToLogin();

  logOut = (): any => this.authService.logout().pipe(takeUntil(this.destroy$)).subscribe()

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
  //#endregion
}
