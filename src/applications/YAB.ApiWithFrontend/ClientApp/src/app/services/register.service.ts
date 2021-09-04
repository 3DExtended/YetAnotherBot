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

  public GetOptionsToFill(): Observable<ErrorHandledResult<List<OptionsDescription>>> {
    return this._httpClient.get<any>(this.baseUrl + "api/register/optionsToFill", {})
      .pipe(errorHandler<List<OptionsDescription>>());
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
  // instance of PropertyValueType
  valueType: number;
}




