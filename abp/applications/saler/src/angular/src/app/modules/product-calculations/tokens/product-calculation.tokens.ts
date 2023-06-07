import { StateToken } from '@ngxs/store';
import { ProductCalculationStateModel } from '../states';

export const PRODUCT_CALCULATION_STATE_TOKEN = new StateToken<ProductCalculationStateModel>('productCalculation');
