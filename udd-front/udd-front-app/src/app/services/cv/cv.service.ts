import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { SearchResult } from './../../models/content-search-result.model';

@Injectable({
  providedIn: 'root'
})
export class CvService {

  constructor(private http: HttpClient) { }

  getByGeoLocation(city:string, radiusKm:number):Observable<SearchResult[]>
  {
    let requestUrl = environment.serverURL.concat("job-documents/location");
    let params = new HttpParams();
    params = params.append('cityName', city);
    params = params.append('radiusKm', radiusKm.toString());

    return this.http.get<SearchResult[]>(requestUrl, {params:params});
  }

  getByEducationLevel(level:number):Observable<SearchResult[]>
  {
    let requestUrl = environment.serverURL.concat("job-documents/education");
    let params = new HttpParams();
    params = params.append('educationLevel', level.toString());

    return this.http.get<SearchResult[]>(requestUrl, {params:params});
  }

  getByNameLastname(name:string, lastName:string):Observable<SearchResult[]>
  {
    let requestUrl = environment.serverURL.concat("job-documents/personal-info");
    let params = new HttpParams();
    params = params.append('name', name);
    params = params.append('lastName', lastName);

    return this.http.get<SearchResult[]>(requestUrl, {params:params});
  }

  getByContent(content:string):Observable<SearchResult[]>
  {
    let requestUrl = environment.serverURL.concat("job-documents/cv-content");
    let params = new HttpParams();
    params = params.append('content', content);

    return this.http.get<SearchResult[]>(requestUrl, {params:params});
  }

  addNewJobApplication(name:string, lastName:string, city:string, educationLevel:number, cvFile:any, coverLetter:any):Observable<boolean>
  {
    let requestUrl = environment.serverURL.concat("job-documents");
    const formData = new FormData();
    formData.append('applicantName', name);
    formData.append('applicantLastname', lastName);
    formData.append('applicantCityName', city);
    formData.append('applicantEducationLevel', educationLevel.toString());
    formData.append('cvFile', cvFile);
    formData.append('coverLetterFile', coverLetter);

    return this.http.post<boolean>(requestUrl, formData);
  }

}
