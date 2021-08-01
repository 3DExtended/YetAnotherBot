import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ErrorHandledResult } from '../shared/errorHandledResult';
import { errorHandler } from '../shared/errorHandler';

@Injectable({
  providedIn: 'root'
})
export class BotStatusService {

  constructor(private readonly _httpClient: HttpClient) { }

  public StartBot(): Observable<any> {
    return this._httpClient.post<any>(environment.baseUrl + "api/botstatus/start", { observe: 'response' }).pipe(errorHandler());
  }

  public IsBotRunning(): Observable<ErrorHandledResult<boolean>> {
    return this._httpClient.get<boolean>(environment.baseUrl + "api/botstatus/status", { observe: 'response' }).pipe(errorHandler());
  }

  public StopBot(): Observable<ErrorHandledResult<any>> {
    return this._httpClient.post<any>(environment.baseUrl + "api/botstatus/stop", { observe: 'response' }).pipe(errorHandler());
  }
}
