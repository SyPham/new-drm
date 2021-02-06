import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Tutorial } from '../_model/tutorial';
import { HierarchyNode, IBuilding } from '../_model/building';
const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json',
    Authorization: 'Bearer ' + localStorage.getItem('token')
  })
};
@Injectable({
  providedIn: 'root'
})
export class BuildingLunchTimeService {
  baseUrl = environment.apiUrlEC;
  messageSource = new BehaviorSubject<number>(0);
  currentMessage = this.messageSource.asObservable();
  // method này để change source message
  changeMessage(message) {
    this.messageSource.next(message);
  }
  constructor(private http: HttpClient) { }
  getBuildings() {
    return this.http.get<Array<IBuilding>>(`${this.baseUrl}BuildingLunchTime/GetAllBuildings`);
  }
  getAllPeriodByLunchTime(lunchTimeID: number) {
    return this.http.get(`${this.baseUrl}BuildingLunchTime/GetAllPeriodByLunchTime/${lunchTimeID}`);
  }
  addOrUpdateLunchTime(item: any) {
    return this.http.post(`${this.baseUrl}BuildingLunchTime/AddOrUpdateLunchTime`, item);
  }
  updatePeriod(item: any) {
    return this.http.post(`${this.baseUrl}BuildingLunchTime/UpdatePeriod`, item);
  }
}
