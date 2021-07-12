import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class BotStatusService {

  constructor(private readonly _httpClient: HttpClient) { }

  public StartBot(): Observable<any> {
    return this._httpClient.post(environment.baseUrl + "api/botstatus/start", {});
    // .pipe(map(res => res['payload']),
    //   catchError(err => {
    //     console.log('caught mapping error and rethrowing', err);
    //     return throwError(err);
    //   }),
    //   finalize(() => console.log("first finalize() block executed")),
    //   catchError(err => {
    //     console.log('caught rethrown error, providing fallback value');
    //     return of([]);
    //   }),
    //   finalize(() => console.log("second finalize() block executed")));
  }

  public IsBotRunning(): Observable<any> {
    return this._httpClient.get(environment.baseUrl + "api/botstatus/status");
    // .pipe(map(res => res['payload']),
    //   catchError(err => {
    //     console.log('caught mapping error and rethrowing', err);
    //     return throwError(err);
    //   }),
    //   finalize(() => console.log("first finalize() block executed")),
    //   catchError(err => {
    //     console.log('caught rethrown error, providing fallback value');
    //     return of([]);
    //   }),
    //   finalize(() => console.log("second finalize() block executed")));
  }

  public StopBot(): Observable<any> {
    return this._httpClient.post(environment.baseUrl + "api/botstatus/stop", {});
    // .pipe(map(res => res['payload']),
    //   catchError(err => {
    //     console.log('caught mapping error and rethrowing', err);
    //     return throwError(err);
    //   }),
    //   finalize(() => console.log("first finalize() block executed")),
    //   catchError(err => {
    //     console.log('caught rethrown error, providing fallback value');
    //     return of([]);
    //   }),
    //   finalize(() => console.log("second finalize() block executed")));
  }
}
