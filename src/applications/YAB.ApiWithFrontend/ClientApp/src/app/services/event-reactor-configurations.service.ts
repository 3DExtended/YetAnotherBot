import { HttpClient, HttpResponse } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ErrorHandledResult } from '../shared/errorHandledResult';
import { errorHandler } from '../shared/errorHandler';
import { List } from './pipelines.service';

@Injectable({
  providedIn: 'root'
})
export class EventReactorConfigurationService {

  constructor(private readonly _httpClient: HttpClient,
    @Inject('API_BASE_URL') private readonly baseUrl: string) { }

  public GetAllEventReactorConfigurations(): Observable<ErrorHandledResult<List<EventReactorConfiguration>>> {
    return this._httpClient.get<HttpResponse<List<EventReactorConfiguration>>>(this.baseUrl + "api/EventReactorConfigurations/all")
      .pipe(errorHandler<List<EventReactorConfiguration>>());;
  }
}

export interface EventReactorConfiguration {
  $type: "YAB.Api.Contracts.Models.EventReactors.EventReactorConfigurationDto, YAB.Api.Contracts",
  eventTypeName: string,
  seralizedEventReactorConfiguration: string
}
