import { StateToken } from '@ngxs/store';
import { OrderStateModel } from '../states';

export const ORDER_STATE_TOKEN = new StateToken<OrderStateModel>('order');

export const ORDER_FILTER_STATE_TOKEN: string = 'order_filter';
