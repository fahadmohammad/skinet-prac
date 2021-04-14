import { Component, OnInit } from '@angular/core';
import { SocialAuthService } from 'angularx-social-login';
import { Observable } from 'rxjs';
import { AccountService } from 'src/app/account/account.service';
import { BasketService } from 'src/app/basket/basket.service';
import { IBasket } from 'src/app/shared/models/basket';
import { IUser } from 'src/app/shared/models/user';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.scss']
})
export class NavBarComponent implements OnInit {

  public isExternalAuth: boolean;
  
  basket$: Observable<IBasket>;
  currentUser$: Observable<IUser>;
  constructor(private basketService: BasketService, private accountService: AccountService,
    private _socialAuthService: SocialAuthService) { }

  ngOnInit(): void {
    this.basket$ = this.basketService.basket$;
    this.currentUser$ = this.accountService.currentUser$;
    this._socialAuthService.authState.subscribe(user => {
      this.isExternalAuth = user != null;
    })
  }

  logOut() {
    this.accountService.logout();
    if(this.isExternalAuth){
      console.log("Logout:- external user: true");
      this.accountService.signOutExternal();
    }
      
  }

}
