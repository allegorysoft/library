import { StateToken } from '@ngxs/store';
import { ItemStateModel } from '../states';

export const ITEM_STATE_TOKEN = new StateToken<ItemStateModel>('item');

export const ITEM_FILTER_STATE_TOKEN: string = 'item_filter';

export const ITEM_LOOKUP_FILTER_STATE_TOKEN: string = 'item_lookup_filter';
