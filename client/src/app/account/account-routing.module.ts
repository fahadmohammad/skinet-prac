import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import {EmailConfirmationComponent} from './email-confirmation/email-confirmation.component';
import { PostRegistrationComponent } from './post-registration/post-registration.component';


const routes: Routes = [
  {path: 'login', component: LoginComponent},
  {path: 'register', component: RegisterComponent},
  {path: 'emailconfirmation', component: EmailConfirmationComponent},
  {path: 'postregistration', component: PostRegistrationComponent}
];
@NgModule({
  declarations: [],
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class AccountRoutingModule { }
