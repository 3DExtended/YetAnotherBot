import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ErrorHandledResult } from '../shared/errorHandledResult';
import { errorHandler } from '../shared/errorHandler';

@Injectable({
  providedIn: 'root'
})
export class PipelinesService {

  constructor(private readonly _httpClient: HttpClient) { }

  public GetRegisteredPipelines(): Observable<ErrorHandledResult<List<IPipelineDto>>> {
    return this._httpClient.get<List<IPipelineDto>>(environment.baseUrl + "api/Pipelines/registered", { observe: 'response' }).pipe(errorHandler());
  }
}


export interface List<T> {
  $values: T[];
  $type: string;
}

export const FilterOperator = {
  "contains": 0,
  "notContains": 1,
  "equals": 2,
  "notEquals": 3
}

export const LogicalOperator = {
  "contains": 0,
  "notContains": 1,
  "equals": 2,
  "notEquals": 3
}

export interface IFilterBase {
  $type: string;
};

export interface IFilter extends IFilterBase {
  $type: "YAB.Core.Pipelines.Filter.Filter, YAB.Core.Pipelines";
  filterValue: string;
  ignoreValueCasing: boolean;
  propertyName: string;

  // instance of FilterOperator
  operator: number;
}

export interface IFilterGroup extends IFilterBase {
  $type: "YAB.Core.Pipelines.Filter.FilterGroup, YAB.Core.Pipelines";

  filters: List<IFilterBase>;

  // instance of LogicalOperator
  operator: number;
}

export interface IEventReactorConfiguration {
  $type: string;
}

export interface IPipelineDto {
  $type: "YAB.Api.Models.Pipelines.PipelineDto, YAB.Api";
  eventFilter: IFilterBase;
  eventName: string;
  eventReactors: any;

  // Those strings are serialized json objects and if we want to extract properties here, we have to deserialize them...
  serializedEventReactorConfiguration: List<string>;
}
