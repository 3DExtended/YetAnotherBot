import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ErrorHandledResult } from '../shared/errorHandledResult';
import { errorHandler } from '../shared/errorHandler';
import { List } from './pipelines.service';

@Injectable({
  providedIn: 'root'
})
export class PluginService {

  constructor(private readonly _httpClient: HttpClient,
    @Inject('API_BASE_URL') private readonly baseUrl: string) { }

  public OfficiallySupportedPlugins(): Observable<ErrorHandledResult<SupportedPluginsDto>> {
    return this._httpClient.get<any>(this.baseUrl + "api/plugins")
      .pipe(errorHandler<SupportedPluginsDto>());
  }

  public InstalledStatusOfSupportedPlugins(): Observable<ErrorHandledResult<List<InstalledPluginTupleDto>>> {
    return this._httpClient.get<any>(this.baseUrl + "api/plugins/installed")
      .pipe(errorHandler<List<InstalledPluginTupleDto>>());
  }

  public GetAllEvents(): Observable<ErrorHandledResult<EventType[]>> {
    return this._httpClient.get<any>(this.baseUrl + "api/plugins/events")
      .pipe(errorHandler<EventType[]>());
  }
}

export interface InstalledPluginTupleDto {
  item1: SupportedPluginDto;
  item2: boolean;
}

export interface EventType {
  $type: string;
  id: string;
  [key: string]: any;
}

export interface SupportedPluginsDto {
  $type: "YAB.Core.Contracts.SupportedPlugins.SupportedPlugins, YAB.Core";
  plugins: List<SupportedPluginDto>;
}

export interface SupportedPluginDto {
  $type: "YAB.Core.Contracts.SupportedPlugins.SupportedPlugin, YAB.Core";
  dllPath: string;
  pluginName: string;
  repositoryUrl: string;
}
