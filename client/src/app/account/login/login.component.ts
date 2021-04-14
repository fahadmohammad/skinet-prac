import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SocialUser } from 'angularx-social-login';
import { externalAuthDto } from 'src/app/shared/models/ExternalAuthDto ';
import { AccountService } from '../account.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  loginForm: FormGroup;
  returnUrl: string;

  constructor(private accountService: AccountService, private router: Router, 
    private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.returnUrl = this.activatedRoute.snapshot.queryParams.returnUrl || '/shop';
    this.createLoginForm();
  }

  createLoginForm() {
    this.loginForm = new FormGroup({
      email: new FormControl('', [Validators.required, Validators
        .pattern('^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$')]),
      password: new FormControl('', Validators.required)
    });
  }
  onSubmit() {
    this.accountService.login(this.loginForm.value).subscribe(() => {
      this.router.navigateByUrl(this.returnUrl);
    }, error => {
      console.log('Error');
    });
  }

  public externalLogin = () => {
    //this.showError = false;
    this.accountService.signInWithGoogle()
    .then(res => {
      const user: SocialUser = { ...res };
      console.log(user);
      const externalAuth: externalAuthDto = {
        provider: user.provider,
        idToken: user.idToken
      }
      this.validateExternalAuth(externalAuth);
    }, error => console.log(error))
  }

  private validateExternalAuth(externalAuth: externalAuthDto) {
    this.accountService.externalLogin(externalAuth).subscribe(() => {
      this.router.navigateByUrl(this.returnUrl);
    }, error => {
      console.log('Error');
      this.accountService.signOutExternal();
    });
  }

}


