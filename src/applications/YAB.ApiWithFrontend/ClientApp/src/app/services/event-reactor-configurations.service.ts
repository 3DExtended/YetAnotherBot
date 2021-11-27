import { HttpClient, HttpResponse } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ErrorHandledResult } from '../shared/errorHandledResult';
import { errorHandler } from '../shared/errorHandler';
import { List } from './pipelines.service';
import { PropertyDescription } from './register.service';

@Injectable({
  providedIn: 'root'
})
export class EventReactorConfigurationService {
  constructor(private readonly _httpClient: HttpClient,
    @Inject('API_BASE_URL') private readonly baseUrl: string) { }

  public GetAllEventReactorConfigurations(): Observable<ErrorHandledResult<List<EventReactorConfiguration>>> {
    return this._httpClient.get<HttpResponse<List<EventReactorConfiguration>>>(this.baseUrl + 'api/EventReactorConfigurations/all')
      .pipe(errorHandler<List<EventReactorConfiguration>>());
  }

  public GetRegisteredPipelineByIdAllowedEventBases(guid: string): Observable<ErrorHandledResult<List<string>>> {
    return this._httpClient.get<List<string>>(this.baseUrl + 'api/EventReactorConfigurations/pipelines/' + guid + '/eventbases', { observe: 'response' })
      .pipe(errorHandler<List<string>>());
  }
}

export interface EventReactorConfiguration {
  $type: 'YAB.Api.Contracts.Models.EventReactors.EventReactorConfigurationDto, YAB.Api.Contracts';
  eventTypeName: string;
  description: string;
  seralizedEventReactorConfiguration: string;
  properties: List<PropertyDescription>;
}
