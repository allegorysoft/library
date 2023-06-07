import type { EntityDto, ExtensibleAuditedEntityDto, ExtensibleEntityDto } from '@abp/ng.core';

export interface GlobalUnitDto {
  code?: string;
  name?: string;
}

export interface UnitCreateDto extends UnitCreateOrUpdateDtoBase {
}

export interface UnitCreateOrUpdateDtoBase extends EntityDto<number> {
  code: string;
  name?: string;
  mainUnit: boolean;
  convFact1: number;
  convFact2: number;
  divisible: boolean;
  globalUnitCode?: string;
}

export interface UnitDto extends EntityDto<number> {
  code?: string;
  name?: string;
  mainUnit: boolean;
  convFact1: number;
  convFact2: number;
  divisible: boolean;
  globalUnitCode?: string;
}

export interface UnitGroupCreateDto extends UnitGroupCreateOrUpdateDtoBase {
  units: UnitCreateDto[];
}

export interface UnitGroupCreateOrUpdateDtoBase extends ExtensibleEntityDto<number> {
  code: string;
  name?: string;
}

export interface UnitGroupDto extends ExtensibleAuditedEntityDto<number> {
  code?: string;
  name?: string;
}

export interface UnitGroupUpdateDto extends UnitGroupCreateOrUpdateDtoBase {
  units: UnitUpdateDto[];
}

export interface UnitGroupWithDetailsDto extends ExtensibleAuditedEntityDto<number> {
  code?: string;
  name?: string;
  units: UnitDto[];
}

export interface UnitUpdateDto extends UnitCreateOrUpdateDtoBase {
  id?: number;
}
