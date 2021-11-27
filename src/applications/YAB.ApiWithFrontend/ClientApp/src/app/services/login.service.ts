import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ErrorHandledResult } from '../shared/errorHandledResult';
import { errorHandler } from '../shared/errorHandler';

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  constructor(private readonly _httpClient: HttpClient,
    @Inject('API_BASE_URL') private readonly baseUrl: string) { }

  public Login(password: string): Observable<ErrorHandledResult<any>> {
    return this._httpClient.post<any>(this.baseUrl + "api/login", { password: password })
      .pipe(errorHandler<any>());;
  }

  public IsRegistrationCompleted(): Observable<ErrorHandledResult<boolean>> {
    return this._httpClient.get<any>(this.baseUrl + "api/login")
      .pipe(errorHandler<boolean>());;
  }
}
