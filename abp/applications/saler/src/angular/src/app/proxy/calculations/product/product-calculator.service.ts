import type { CalculableProductAggregateRootDto, CalculableProductAggregateRootInputDto, CalculableProductDto, CalculableProductInputDto, DeductionDto } from './models';
import { RestService } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ProductCalculatorService {
  apiName = 'Default';

  calculate = (input: CalculableProductInputDto) =>
    this.restService.request<any, CalculableProductDto>({
      method: 'POST',
      url: '/api/app/product-calculator/calculate',
      body: input,
    },
      { apiName: this.apiName });

  calculateAggregateRoot = (input: CalculableProductAggregateRootInputDto) =>
    this.restService.request<any, CalculableProductAggregateRootDto>({
      method: 'POST',
      url: '/api/app/product-calculator/calculate-aggregate-root',
      body: input,
    },
      { apiName: this.apiName });

  getDeductions = () =>
    this.restService.request<any, DeductionDto[]>({
      method: 'GET',
      url: '/api/app/product-calculator/deductions',
    },
      { apiName: this.apiName });

  constructor(private restService: RestService) { }
}
