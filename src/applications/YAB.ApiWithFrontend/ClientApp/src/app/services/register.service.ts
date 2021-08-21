import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ErrorHandledResult } from '../shared/errorHandledResult';
import { errorHandler } from '../shared/errorHandler';

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
    debugger;
    return this._httpClient.post<any>(this.baseUrl + "api/register/addplugin?extensionName=" + pluginName, {})
      .pipe(errorHandler<boolean>());
  }
}




