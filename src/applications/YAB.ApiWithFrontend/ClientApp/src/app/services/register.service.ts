import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ErrorHandledResult } from '../shared/errorHandledResult';
import { errorHandler } from '../shared/errorHandler';
import { List } from './pipelines.service';

@Injectable({
  providedIn: 'root'
})
export class RegisterService {
  constructor(private readonly _httpClient: HttpClient,
    @Inject('API_BASE_URL') private readonly baseUrl: string) { }

  public Register(password: string): Observable<ErrorHandledResult<any>> {
    return this._httpClient.post<any>(this.baseUrl + "api/register", { password: password })
      .pipe(errorHandler<any>());
  }

  public InstallPlugin(pluginName: string): Observable<ErrorHandledResult<boolean>> {
    return this._httpClient.post<any>(this.baseUrl + "api/register/addplugin?extensionName=" + pluginName, {})
      .pipe(errorHandler<boolean>());
  }

  public GetOptionsToFill(password: string): Observable<ErrorHandledResult<List<OptionsDescription>>> {
    return this._httpClient.get<any>(this.baseUrl + "api/register/optionsToFill?password=" + password, {})
      .pipe(errorHandler<List<OptionsDescription>>());
  }

  public UpdateOptionsToFill(password: string, updatedOptions: OptionsDescription[]): Observable<ErrorHandledResult<any>> {
    const result: any[] = [];
    updatedOptions.forEach(option => {
      result.push({
        "optionFullName": option.optionFullName,
        "updatedProperties": option.properties.$values.map(v => {
          return {
            "propertyName": v.propertyName,
            "stringifiedPropertyValue": v.value
          };
        })
      });
    });

    return this._httpClient.post<any>(this.baseUrl + "api/register/options?botPassword=" + password, result, {})
      .pipe(errorHandler<any>());
  }
}

export interface OptionsDescription {
  $type: "YAB.Api.Contracts.Models.Plugins.OptionDescriptions.OptionsDescriptionDto, YAB.Api.Contracts",
  optionFullName: string;
  properties: List<PropertyDescription>;
}

export const PropertyValueType = {
  "string": 0,
  "int": 1,
  "floatingPoint": 2,
}

export interface PropertyDescription {
  $type: "YAB.Api.Contracts.Models.Plugins.OptionDescriptions.PropertyDescriptionDto, YAB.Api.Contracts",
  propertyDescription: string;
  propertyName: string;
  isSecret: boolean;
  currentValue: string;
  // instance of PropertyValueType
  valueType: number;

  value: any;
}
