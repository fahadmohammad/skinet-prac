import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { AccountRoutingModule } from './account-routing.module';
import { SharedModule } from '../shared/shared.module';
import { EmailConfirmationComponent } from './email-confirmation/email-confirmation.component';
import { PostRegistrationComponent } from './post-registration/post-registration.component';



@NgModule({
  declarations: [LoginComponent, RegisterComponent, EmailConfirmationComponent, PostRegistrationComponent],
  imports: [
    CommonModule,
    AccountRoutingModule,
    SharedModule
  ]
})
export class AccountModule { }
