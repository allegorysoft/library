import { StateToken } from '@ngxs/store';
import { CurrencyStateModel } from '../states';

export const CURRENCY_STATE_TOKEN = new StateToken<CurrencyStateModel>('currency');

export const CURRENCY_FILTER_STATE_TOKEN: string = 'currency_filter';

export const DAILY_EXCHANGE_FILTER_STATE_TOKEN: string = 'daily_exchange_filter';
