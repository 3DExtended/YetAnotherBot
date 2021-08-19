import { HttpClient, HttpResponse } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ErrorHandledResult } from '../shared/errorHandledResult';
import { errorHandler } from '../shared/errorHandler';
import { List } from './pipelines.service';

@Injectable({
  providedIn: 'root'
})
export class EventService {

  constructor(private readonly _httpClient: HttpClient,
    @Inject('API_BASE_URL') private readonly baseUrl: string) { }

  public EventsOfPast24Hours(): Observable<ErrorHandledResult<List<EventLoggingEntryDto>>> {
    return this._httpClient.get<HttpResponse<List<EventLoggingEntryDto>>>(this.baseUrl + "api/botevents/past24hours")
      .pipe(errorHandler<List<EventLoggingEntryDto>>());;
  }
}

export interface EventLoggingEntryDto {
  eventDescription: string;
  eventGroup: string;
  eventName: string;
  timeOfEvent: string;
}
