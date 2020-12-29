import { CoreEnvironment } from '@angular/compiler/src/compiler_facade_interface';
import {environment} from 'src/environments/environment'
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IDeliveryMethod } from '../shared/models/DeliveryMethod';
import { map } from 'rxjs/operators';
import { IOrderToCreate } from '../shared/models/order';

@Injectable({
  providedIn: 'root'
})
export class CheckoutService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }
  createOrder(order: IOrderToCreate) {
    return this.http.post(this.baseUrl + 'order', order);
  }
  
  getDeliveryMethods() {
    return this.http.get(this.baseUrl + 'order/deliveryMethods').pipe(
      map((dm: IDeliveryMethod[]) => {
        return dm.sort((a,b) => b.price - a.price);
      })
    );
  }
}
