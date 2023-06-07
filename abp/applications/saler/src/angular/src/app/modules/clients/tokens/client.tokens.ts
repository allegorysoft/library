import { StateToken } from '@ngxs/store';
import { ClientStateModel } from '../states';

export const CLIENT_STATE_TOKEN = new StateToken<ClientStateModel>('client');

export const CLIENT_FILTER_STATE_TOKEN: string = 'client_filter';
