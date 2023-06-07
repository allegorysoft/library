import type { ExtensibleAuditedEntityDto, ExtensibleEntityDto } from '@abp/ng.core';

export interface ClientCreateDto extends ClientCreateOrUpdateDtoBase {
}

export interface ClientCreateOrUpdateDtoBase extends ExtensibleEntityDto {
  code: string;
  title?: string;
  identityNumber?: string;
}

export interface ClientDto extends ExtensibleAuditedEntityDto<number> {
  code?: string;
  title?: string;
  identityNumber?: string;
}

export interface ClientUpdateDto extends ClientCreateOrUpdateDtoBase {
}
