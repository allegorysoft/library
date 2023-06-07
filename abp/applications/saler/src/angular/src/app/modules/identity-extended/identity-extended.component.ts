import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { IdentityUserDto } from '@abp/ng.identity/proxy';
import { Confirmation, ConfirmationService, ToasterService } from '@abp/ng.theme.shared';
import { ClientUserService } from '@proxy/client-users';
import { ClientDto } from '@proxy/clients';

@Component({
  selector: 'app-identity-extended',
  templateUrl: './identity-extended.component.html',
})
export class IdentityExtendedComponent implements OnInit, OnDestroy {
  //#region Fields
  subscription: Subscription;
  isUserQuickViewVisible: boolean;

  user: IdentityUserDto;
  visible: boolean = false;
  busy: boolean = false;
  //#endregion

  //#region Ctor
  constructor(
    private clientUserService: ClientUserService,
    private toasterService: ToasterService,
    private confirmation: ConfirmationService
  ) { }
  //#endregion

  //#region Methods
  ngOnInit(): void {
  }

  openUserQuickView(record: IdentityUserDto) {
    this.user = new Proxy(record, {
      get: (target, prop) => target[prop] || '—',
    });
  }

  openClientList(record: IdentityUserDto): void {
    this.user = record;
    this.visible = true;
  }

  clientAfterSelect(client: ClientDto): void {
    if (client) {
      this.subscription?.unsubscribe();

      this.subscription = this.clientUserService
        .addUserByClientIdAndUserId(client.id, this.user.id)
        .subscribe(() => {
          this.toasterService.success('::CreatedSuccessfullyMessage', '::SuccessTitleMessage');
          this.visible = false;
        });
    }
  }

  clear(): void {
    this.subscription?.unsubscribe();
    this.busy = true;

    const options: Partial<Confirmation.Options> = {
      messageLocalizationParams: [this.user.name]
    };

    this.subscription = this.confirmation
      .warn('::ClientDisconnectionConfirmationMessage', '::AreYouSure', options)
      .subscribe((response: Confirmation.Status) => {
        if (response == Confirmation.Status.confirm) {
          this.subscription = this.clientUserService
            .removeUserByUserId(this.user.id)
            .pipe(finalize(() => this.busy = false))
            .subscribe(() => {
              this.toasterService.success('::RemovedSuccessfullyMessage', '::SuccessTitleMessage');
            });
        }
        else this.busy = false;
      });
  }

  onHide = (): any => {
    this.subscription?.unsubscribe();
    this.busy = false;
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }
  //#endregion
}