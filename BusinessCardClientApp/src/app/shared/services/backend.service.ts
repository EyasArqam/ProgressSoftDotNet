import {
  BaseResponse,
  PostResponse,
  PutResponse,
  GetResponse,
  DeleteResponse
} from '../../data/models/response';
import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpErrorResponse,

} from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class BackendService {

  get baseUrl(): string {
    return 'https://localhost:7281/api/';
  }

  constructor(private http: HttpClient) { }


  get<T = {}>(url: string): Promise<GetResponse<T> | any> {
    return this.http
      .get<GetResponse<T>>(this.baseUrl + url)
      .pipe(
        map((res) => new GetResponse({ ok: true, body: res })),
        catchError(this.handleError<any>(url, []))
      )
      .toPromise();
  }

  post<T = {}>(url: string, obj: any): Promise<PostResponse<T>> {
    return this.http
      .post<any>(this.baseUrl + url, obj)
      .pipe(
        map((res) => new PostResponse<T>({ ok: true, body: res })),
        catchError(this.handleError<any>(url, []))
      )
      .toPromise();

  }

  postFiles<T = {}>(url: string, files: any): Promise<PostResponse<T>> {
    const formData = new FormData();

    formData.append('file', files[0], files[0].name);

    return this.http
      .post<any>(`${this.baseUrl}${url}`, formData)
      .pipe(
        map(res => new PostResponse<T>({ ok: true, body: res })),
        catchError(this.handleError<any>(url, []))
      )
      .toPromise();
  }


  delete(url: string): Promise<DeleteResponse | any> {
    return this.http
      .delete<DeleteResponse>(this.baseUrl + url)
      .pipe(
        map((res) => new DeleteResponse({ ok: true})),
        catchError(this.handleDeleteError(url))
      )
      .toPromise();
  }

  ExportXml<T = {}>(url: string, id: number): Promise<any> {
    return this.http
      .get(this.baseUrl + `${url}/${id}`, { responseType: 'blob' })
      .pipe(
        map((res: Blob) => {
          this.downloadFile(res, 'BusinessCard.xml');
          return new GetResponse({ ok: true, body: res });
        }),
        catchError(this.handleError<any>(`${url}/${id}`, []))
      )
      .toPromise();
  }

  ExportCsv<T = {}>(url: string, id: number): Promise<any> {
    return this.http
      .get(this.baseUrl + `${url}/${id}`, { responseType: 'blob' })
      .pipe(
        map((res: Blob) => {
          this.downloadFile(res, 'BusinessCard.csv');
          return new GetResponse({ ok: true, body: res });
        }),
        catchError(this.handleError<any>(`${url}/${id}`, []))
      )
      .toPromise();
  }

  getObservable(url: string): Observable<any> {
    url = url.replace('null', '');
    return this.http
      .get<any>(this.baseUrl + url)
      .pipe(catchError(this.handleError<any>(url, [])));
  }

  private downloadFile(data: Blob, filename: string) {
    const url = window.URL.createObjectURL(data);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.click();
    window.URL.revokeObjectURL(url);
  }

  private handleDeleteError(
    operation = 'operation',
    result?: DeleteResponse
  ): (res: any) => Observable<DeleteResponse> {
    return (res: HttpErrorResponse): Observable<DeleteResponse> => {
      let error: DeleteResponse;

      if (res?.error?.ModelState) {
        error = Object.keys(res.error.ModelState).map((x) => {
          return new DeleteResponse({
            ok: false,
            ErrorCode: x,
            Message: res.error.ModelState[x][0],
            Status: res.status,
            StatusText: res.statusText,
          });
        })[0];
      } else {
        error = new DeleteResponse({
          ok: false,
          ErrorCode: 'UnknownError',
          Message: 'An unknown error occurred',
          Status: res.status,
          StatusText: res.statusText,
        });
      }

      this.log(error);
      return of((result || error) as DeleteResponse);
    };
  }

  private handleError<T>(
    operation = 'operation',
    result?: T
  ): (res: any) => Observable<T> {
    return (res: HttpErrorResponse): Observable<T> => {
      let error: BaseResponse;

      if (res?.error?.ModelState) {
        error = Object.keys(res.error.ModelState).map((x) => {
          return new BaseResponse({
            ok: false,
            ErrorCode: x,
            Message: res.error.ModelState[x][0] || res.error?.error,
            Status: res.status,
            StatusText: res.statusText,
          });
        })[0];
        this.log(error);
        return of(error as unknown as T);
      }

      if (res.error?.error) {
        const message = typeof res.error.error === 'string'
          ? res.error.error
          : 'Unknown error occurred';

        error = new BaseResponse({
          ok: false,
          ErrorCode: 'UnknownError',
          Message: message,
          Status: res.status,
          StatusText: res.statusText,
        });
        this.log(error);
        return of(error as unknown as T);
      }

      if (!res.ok) {
        error = new BaseResponse({
          ok: false,
          ErrorCode: 'BadRequest',
          Message: res.message || 'Bad request error',
          Status: res.status,
          StatusText: res.statusText,
        });
        this.log(error);
        return of(error as unknown as T);
      }


      

      return of(result as T);
    };
  }

  private log(message: any): void {
    console.log(message);
  }
}
