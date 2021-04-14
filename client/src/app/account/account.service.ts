import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, of, ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IAddress } from '../shared/models/address';
import { IDeliveryMethod } from '../shared/models/DeliveryMethod';
import { IUser } from '../shared/models/user';
import { SocialAuthService } from "angularx-social-login";
import { GoogleLoginProvider } from "angularx-social-login";
import { externalAuthDto } from '../shared/models/ExternalAuthDto ';
import { exAuthResponseDto } from '../shared/models/exAuthResponseDto';
import { CustomEncoder } from '../shared/customEncoder';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient, private router: Router,
    private _externalAuthService: SocialAuthService) { } 
 
  loadCurrentUser(token: string) {
    if(token === null) {
      this.currentUserSource.next(null);
      return of(null);
    }
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', `Bearer ${token}`);
    return this.http.get(this.baseUrl + 'account', {headers}).pipe(
      map((user: IUser) => {
        if (user) {
          localStorage.setItem('token', user.token);
          this.currentUserSource.next(user);
        }
      })
    )
  }

  login(values: any) {
    return this.http.post(this.baseUrl + 'account/login', values).pipe(
      map((user: IUser) => {
        if (user) {
          localStorage.setItem('token', user.token);
          this.currentUserSource.next(user);
        }
      })
    );
  }

  externalLogin(exAuthDto: externalAuthDto){
    return this.http.post(this.baseUrl+ 'account/externallogin',exAuthDto).pipe(
      map((user: IUser) => {
        if (user) {
          localStorage.setItem('token', user.token);
          this.currentUserSource.next(user);
        }
      })
    );
  }
  
  public signInWithGoogle = ()=> {
    return this._externalAuthService.signIn(GoogleLoginProvider.PROVIDER_ID);
  }
  
  public signOutExternal = () => {
    this._externalAuthService.signOut();
  }


  // register(values: any) {
  //   return this.http.post(this.baseUrl + 'account/register', values).pipe(
  //     map((user: IUser) => {
  //       if (user) {
  //         localStorage.setItem('token', user.token);
  //         this.currentUserSource.next(user);
  //       }
  //     })
  //   );
  // }

  register(values: any) {
    return this.http.post(this.baseUrl + 'account/register', values)
  }

  confirmEmail(token: string, email: string) {
    let params = new HttpParams({ encoder: new CustomEncoder()})
    params = params.append('token', token);
    params = params.append('email', email);
    
    return this.http.get(this.baseUrl + 'account/emailconfirmation', {params: params}).pipe(
      map((user: IUser) => {
        if (user) {
          localStorage.setItem('token', user.token);
          this.currentUserSource.next(user);
        }
      })
    );
  }

  logout() {
    localStorage.removeItem('token');
    this.currentUserSource.next(null);
    this.router.navigateByUrl('/');
  }

  checkEmailExist(email: string) {
    return this.http.get(this.baseUrl + 'account/emailexists?email=' + email);
  }

  getUserAddress() {
    return this.http.get<IAddress>(this.baseUrl + 'account/address');
  }

  updateUserAddress(address: IAddress) {
    return this.http.put<IAddress>(this.baseUrl + 'account/address',address); 
  }

}
