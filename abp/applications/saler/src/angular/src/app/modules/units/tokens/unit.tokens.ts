import { StateToken } from '@ngxs/store';
import { UnitStateModel } from '../states';

export const UNIT_STATE_TOKEN = new StateToken<UnitStateModel>('unit');

export const UNIT_GROUP_FILTER_STATE_TOKEN: string = 'unit_group_filter';
