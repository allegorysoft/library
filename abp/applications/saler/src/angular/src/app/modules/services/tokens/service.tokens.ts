import { StateToken } from '@ngxs/store';
import { ServiceStateModel } from '../states';

export const SERVICE_STATE_TOKEN = new StateToken<ServiceStateModel>('service');

export const SERVICE_FILTER_STATE_TOKEN: string = 'service_filter';

export const SERVICE_LOOKUP_FILTER_STATE_TOKEN: string = 'service_lookup_filter';
