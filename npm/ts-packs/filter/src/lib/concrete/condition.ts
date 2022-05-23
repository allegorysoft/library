import type { Operator } from '../enums/operator.enum';

export interface Condition {
  column?: string;
  operator: Operator;
  value: any;
  not: boolean;
  group: Condition[];
  groupOr: boolean;
  parameterName?: string;
  parent: Condition;
  isColumn: boolean;
  isGroup: boolean;
}
