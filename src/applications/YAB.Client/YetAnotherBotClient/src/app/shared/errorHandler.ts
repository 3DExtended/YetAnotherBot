import { HttpErrorResponse, HttpResponse } from "@angular/common/http";
import { Observable, of } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { ErrorHandledResult } from "./errorHandledResult";
import { ErrorKind } from "./errorKind";

export type ApiCall<T> = () => Observable<T>;

export function errorHandler<T>(statusCodesToIgnore?: number[]) {
  return (source: Observable<HttpResponse<T>>) =>
    source.pipe(
      map((value) => new ErrorHandledResult<T>(value.body as T, true, 0, null)),
      /*map<T, ErrorHandledResult<T>>((value) => new ErrorHandledResult<T>(value, true, 0, null) as any),
      catchError((err: ErrorHandledResult<T>, caught: Observable<T>) => {
        const error = err as any;
        if (!error) {
          return of(new ErrorHandledResult<T>(error.result, true, error.status, null));;
        }
        if (statusCodesToIgnore && statusCodesToIgnore.some(s => s === error.status)) {
          return of(new ErrorHandledResult<T>(error.result, false, error.status, ErrorKind.Client));
        }

        return new Observable((observer) => {
          const resolvedPromise = Promise.all([]);

          resolvedPromise.then(result => {
            observer.next(new ErrorHandledResult<T>(null, false, error.status, ErrorKind.Fatal));
            observer.complete();
          });
        });
      }),
      map((value: ErrorHandledResult<T>) => value));*/
      catchError((error: HttpErrorResponse): Observable<ErrorHandledResult<T>> => {
        return of(new ErrorHandledResult<T>({} as T, false, error.status, ErrorKind.Fatal));
      })
    );
}
