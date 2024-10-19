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


  getAll(
    Url: string,
  ): Promise<any[] | undefined> {
    return this.http
      .get<any[]>(`${this.baseUrl}/${Url}`)
      .pipe(catchError(this.handleError<any[]>(Url, [])))
      .toPromise();
  }

  get<T = {}>(url: string): Promise<GetResponse<T> | any> {
    return this.http
      .get<GetResponse<T>>(this.baseUrl + url)
      .pipe(
        map((res) => new GetResponse({ ok: true, body: res })),
        catchError(this.handleError<any>(url, []))
      )
      .toPromise();
  }

  getFull<T = {}>(url: string): Promise<GetResponse<T> | any> {
    return this.http
      .get<GetResponse<T>>(url)
      .pipe(
        map((res) => new GetResponse({ ok: true, body: res })),
        catchError(this.handleError<any>(url, []))
      )
      .toPromise();
  }

  put(url: string, obj: { [x: string]: any; }, files?: any[]): Promise<any> {
    let HasUploadFile = false;

    if (!(obj instanceof Array)) {
      Object.keys(obj).forEach((key) => {
        if (
          obj[key] &&
          obj[key].Id &&
          typeof obj[key] === 'object' &&
          !(obj[key] instanceof Date) &&
          !(obj[key] instanceof Array) &&
          !(obj[key] instanceof File)
        ) {
          obj[key] = obj[key].Id;
        }
        if (key === 'Id' && !obj[key]) {
          obj[key] = undefined;
        } else if (obj[key] instanceof Date) {
          const Offset = (new Date(obj[key]).getTimezoneOffset() * -1) / 60;
          const date: Date = new Date(obj[key]);
          date.setHours(date.getHours() + Number(Offset));
          obj[key] = date;
        }
        if (obj[key] instanceof File) {
          HasUploadFile = true;
        }
        if (files) {
          if (obj[key] && obj[key][0] instanceof File) {
            HasUploadFile = true;
          }
        }
      });
    }

    if (HasUploadFile || files) {
      const formdata = new FormData();
      if (obj instanceof Array) {
        formdata.append('data', JSON.stringify(obj));
      } else {
        Object.keys(obj).forEach((key) => {
          if (!(obj[key] instanceof Array)) {
            formdata.append(key, obj[key]);
          } else {
            formdata.append(key, JSON.stringify(obj[key]));
          }
        });
      }

      if (files) {
        files.forEach((file, index) => {
          formdata.append('file_' + index, file);
        });
      }
      return this.http
        .put<any>(this.baseUrl + url, formdata)
        .pipe(
          map((res) => new PutResponse({ ok: true, ...res })),
          catchError(this.handleError<any>(url, []))
        )
        .toPromise();
    } else {
      return this.http
        .put<any>(this.baseUrl + url, obj)
        .pipe(
          map((res) => new PutResponse({ ok: true, ...res })),
          catchError(this.handleError<any>(url, []))
        )
        .toPromise();
    }
  }

  putFormData(url: string, obj: { [x: string]: any; }, files?: any[]): Promise<any> {
    let HasUploadFile = false;

    if (!(obj instanceof Array)) {
      Object.keys(obj).forEach((key) => {
        if (
          obj[key] &&
          obj[key].Id &&
          typeof obj[key] === 'object' &&
          !(obj[key] instanceof Date) &&
          !(obj[key] instanceof Array) &&
          !(obj[key] instanceof File)
        ) {
          obj[key] = obj[key].Id;
        }
        if (key === 'Id' && !obj[key]) {
          obj[key] = undefined;
        } else if (obj[key] instanceof Date) {
          const Offset = (new Date(obj[key]).getTimezoneOffset() * -1) / 60;
          const date: Date = new Date(obj[key]);
          date.setHours(date.getHours() + Number(Offset));
          obj[key] = date;
        }
        if (obj[key] instanceof File) {
          HasUploadFile = true;
        }
        if (files) {
          if (obj[key] && obj[key][0] instanceof File) {
            HasUploadFile = true;
          }
        }
      });
    }

    const formdata = new FormData();
    if (obj instanceof Array) {
      formdata.append('data', JSON.stringify(obj));
    } else {
      Object.keys(obj).forEach((key) => {
        if (!(obj[key] instanceof Array)) {
          formdata.append(key, obj[key]);
        } else {
          formdata.append(key, JSON.stringify(obj[key]));
        }
      });
    }

    if (files) {
      files.forEach((file, index) => {
        formdata.append('file_' + index, file);
      });
    }
    return this.http
      .put<any>(this.baseUrl + url, formdata)
      .pipe(
        map((res) => new PutResponse({ ok: true, ...res })),
        catchError(this.handleError<any>(url, []))
      )
      .toPromise();
  }


  post<T = {}>(url: string, obj: { [x: string]: any; }, files?: any[]): Promise<PostResponse<T>> {
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

  postFormData<T = {}>(url: string, obj: { [x: string]: any; }, files?: any[]): Promise<PostResponse<T>> {
    let HasUploadFile = false;

    if (!(obj instanceof Array)) {
      Object.keys(obj).forEach((key) => {
        if (
          obj[key] &&
          obj[key].Id &&
          typeof obj[key] === 'object' &&
          !(obj[key] instanceof Date) &&
          !(obj[key] instanceof Array) &&
          !(obj[key] instanceof File)
        ) {
          obj[key] = obj[key].Id;
        }
        if (key === 'Id' && !obj[key]) {
          obj[key] = undefined;
        } else if (obj[key] instanceof Date) {
          const Offset = (new Date(obj[key]).getTimezoneOffset() * -1) / 60;
          const date: Date = new Date(obj[key]);
          date.setHours(date.getHours() + Number(Offset));
          obj[key] = date;
        }
        if (obj[key] instanceof File) {
          HasUploadFile = true;
        }
        if (files) {
          if (obj[key] && obj[key][0] instanceof File) {
            HasUploadFile = true;
          }
        }
      });
    }

    const formdata = new FormData();
    if (obj instanceof Array) {
      formdata.append('data', JSON.stringify(obj));
    } else {
      Object.keys(obj).forEach((key) => {
        if (!(obj[key] instanceof Array)) {
          formdata.append(key, obj[key]);
        } else {
          formdata.append(key, JSON.stringify(obj[key]));
        }
      });
    }

    if (files) {
      files.forEach((file, index) => {
        formdata.append('file_' + index, file);
      });
    }

    return this.http
      .post<any>(this.baseUrl + url, formdata)
      .pipe(
        map((res) => new PostResponse({ ok: true, body: res })),
        catchError(this.handleError<any>(url, []))
      )
      .toPromise();
  }

  delete(url: string): Promise<DeleteResponse | undefined> {
    return this.http
      .delete<DeleteResponse>(this.baseUrl + url)
      .pipe(
        map((res) => new DeleteResponse({ ...res })),
        catchError(this.handleDeleteError(url, undefined))
      )
      .toPromise();
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
