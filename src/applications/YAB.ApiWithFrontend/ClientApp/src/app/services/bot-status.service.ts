import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ErrorHandledResult } from '../shared/errorHandledResult';
import { errorHandler } from '../shared/errorHandler';

@Injectable({
  providedIn: 'root'
})
export class BotStatusService {
  constructor(private readonly _httpClient: HttpClient,
    @Inject('API_BASE_URL') private readonly baseUrl: string) { }

  public StartBot(): Observable<any> {
    return this._httpClient.post<any>(this.baseUrl + "api/botstatus/start", { observe: 'response' }).pipe(errorHandler());
  }

  public IsBotRunning(): Observable<ErrorHandledResult<boolean>> {
    return this._httpClient.get<boolean>(this.baseUrl + "api/botstatus/status", { observe: 'response' }).pipe(errorHandler());
  }

  public StopBot(): Observable<ErrorHandledResult<any>> {
    return this._httpClient.post<any>(this.baseUrl + "api/botstatus/stop", { observe: 'response' }).pipe(errorHandler());
  }
}
