import { ErrorKind } from './errorKind';

export class ErrorHandledResult<T> {
  readonly data: T;
  readonly successful: boolean;
  readonly statusCode: number;
  readonly errorKind: ErrorKind | null;

  constructor(data: T, successful: boolean, statusCode: number, errorKind: ErrorKind | null) {
    this.data = data;
    this.successful = successful;
    this.errorKind = errorKind;
    this.statusCode = statusCode;
  }

  public static successful<T>(): ErrorHandledResult<T> {
    return new ErrorHandledResult<T>({} as T, true, 200, null);
  }

  public static failed<T>(): ErrorHandledResult<T> {
    return new ErrorHandledResult<T>({} as T, false, 0, ErrorKind.Fatal);
  }
}
