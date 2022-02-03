import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { City } from './../../models/city.model';

@Injectable({
  providedIn: 'root'
})
export class CityService {

  constructor(private http: HttpClient) { }

  getCities(): Observable<City[]> {
    let requestUrl = environment.serverURL.concat(`cities`);
		return this.http.get<City[]>(requestUrl);
  }
}
