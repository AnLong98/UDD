import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { JobApplication } from 'src/app/models/job-application.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CvService {

  constructor(private http: HttpClient) { }

  getByGeoLocation(city:string, radiusKm:number):Observable<JobApplication[]>
  {
    let requestUrl = environment.serverURL.concat("job-documents/location");
    let params = new HttpParams();
    params = params.append('cityName', city);
    params = params.append('radiusKm', radiusKm.toString());

    return this.http.get<JobApplication[]>(requestUrl, {params:params});
  }
}
