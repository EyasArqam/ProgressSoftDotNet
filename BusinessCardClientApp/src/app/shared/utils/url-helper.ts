import { HttpParams } from "@angular/common/http";

export class UrlHelper {


    static toUrlwithParams(url: string, data: any) {
        Object.keys(data).forEach((key) => {
            if (
                data[key] &&
                data[key].Id &&
                typeof data[key] == 'object' &&
                !(data[key] instanceof Date) &&
                !(data[key] instanceof Array) &&
                !(data[key] instanceof File)
            ) {
                data[key] = data[key].Id;
            }
        });
        const params = new HttpParams({ fromObject: data });
        return url + '?' + params.toString();
    }

}
