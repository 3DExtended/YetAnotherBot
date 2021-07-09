import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class LoginService {

  constructor(private readonly _httpClient: HttpClient) { }

  public Login(password: string): Observable<any> {
    return this._httpClient.post(environment.baseUrl + "api/login", { password: password });
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




