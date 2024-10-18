
export class BaseResponse {
  constructor(data: BaseResponse) {
    Object.assign(this, data);
  }
  readonly Status?: number;
  readonly StatusText?: string;
  readonly ok: boolean = false;
  readonly ErrorCode?: string;
  readonly Message?: string;
}

export class GetResponse<T> extends BaseResponse {
  readonly body: T | undefined;
  constructor(data: GetResponse<T>) {
    super(data);
    Object.assign(this, data);
  }
}

export class ListResponse<T> extends BaseResponse {
  readonly body: T[] = [];
}

export class PostResponse<T> extends BaseResponse {
  readonly Id?: number;
  readonly body: T | undefined;
  constructor(data: PostResponse<T>) {
    super(data);
    Object.assign(this, data);
  }
}
export class PutResponse<T> extends BaseResponse {
  readonly Id: number | undefined;
  readonly body: T | undefined;
  constructor(data: PutResponse<T>) {
    super(data);
    Object.assign(this, data);
  }
}

export class DeleteResponse extends BaseResponse {
  constructor(data: DeleteResponse) {
    super(data);
    Object.assign(this, data);
  }
}



