import { StateToken } from '@ngxs/store';
import { UnitPriceStateModel } from '../states';

export const UNIT_PRICE_STATE_TOKEN = new StateToken<UnitPriceStateModel>('unitPrice');

export const UNIT_PRICE_FILTER_STATE_TOKEN: string = 'unit_price_filter';
