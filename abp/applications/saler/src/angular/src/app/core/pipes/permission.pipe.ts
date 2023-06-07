import { NgModule, Pipe, PipeTransform } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PermissionService } from '@abp/ng.core';
import { isEmptyOrSpaces } from 'src/app/shared/utils';

@Pipe({
  name: 'permission'
})
export class PermissionPipe implements PipeTransform {

  constructor(private permissionService: PermissionService) { }

  transform(value: string): boolean {
    if (value && !isEmptyOrSpaces(value))
      return this.permissionService.getGrantedPolicy(value);
    return false;
  }
}

@NgModule({
  declarations: [PermissionPipe],
  imports: [CommonModule],
  exports: [PermissionPipe],
})
export class PermissionPipeModule { }
