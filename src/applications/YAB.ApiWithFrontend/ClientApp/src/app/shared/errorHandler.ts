import { HttpErrorResponse, HttpResponse } from "@angular/common/http";
import { Observable, of } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { ErrorHandledResult } from "./errorHandledResult";
import { ErrorKind } from "./errorKind";

export type ApiCall<T> = () => Observable<T>;

export function errorHandler<T>() {
  return (source: Observable<HttpResponse<T>>) =>
    source.pipe(
      map((value) => {
        if (value?.body) {
          return new ErrorHandledResult<T>(value?.body as T, true, 0, null);
        } else {
          return new ErrorHandledResult<T>(value as unknown as T, true, 0, null);
        }
      }),
      catchError((error: HttpErrorResponse): Observable<ErrorHandledResult<T>> => {
        return of(new ErrorHandledResult<T>({} as T, false, error.status, ErrorKind.Fatal));
      })
    );
}
