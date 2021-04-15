import { Component, OnInit } from '@angular/core';
import { AsyncValidatorFn, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { of, timer } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { AccountService } from '../account.service';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent implements OnInit {

  forgotPasswordForm: FormGroup;
  error: string;
  messages: string;
  
  constructor(private fb: FormBuilder, private accountService: AccountService, 
      private router: Router, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.createForgotPasswordForm();
  }

  createForgotPasswordForm() {
    this.forgotPasswordForm = this.fb.group({     
      email: [null, [Validators.required, Validators
        .pattern('^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$')],
        [this.validateEmailNotTaken()]],      
      clientUri: environment.passwordResetCallback
    });
  }

  onSubmit() {
    this.accountService.forgotPassword(this.forgotPasswordForm.value).subscribe(response => {
      this.messages = "An email is sent with the password reset link";
    }, error => {
      console.log(error);
      this.error = error.error;
      this.toastr.error(error.error);
    });
  }

  validateEmailNotTaken(): AsyncValidatorFn {
    return control => {
      return timer(500).pipe(
        switchMap(() => {
          if(!control.value) {
            return of(null);
          }
          return this.accountService.checkEmailExist(control.value).pipe(
            map(res => {
              return !res ? {emailNotExists:true} : null;
            })
          );
        })
      );
    };
  }

}
