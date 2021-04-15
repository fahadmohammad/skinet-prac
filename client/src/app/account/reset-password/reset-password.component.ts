import { Component, OnInit } from '@angular/core';
import { AsyncValidatorFn, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { timer } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { AccountService } from '../account.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit {

  resetPasswordForm: FormGroup;
  errors: string[];
  message: string;

  constructor(private fb: FormBuilder, private accountService: AccountService, 
      private router: Router, private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.createPasswordForm();
  }

  createPasswordForm() {
    this.resetPasswordForm = this.fb.group({     
      password: [null, [Validators.required]],
      confirmPassword: [null, [Validators.required]],
      token: this.activatedRoute.snapshot.queryParams['token'],
      email: this.activatedRoute.snapshot.queryParams['email']
    });
  }

  onSubmit() {
    this.accountService.resetPassword(this.resetPasswordForm.value).subscribe(response => {
      this.message = "Password reseted.Please login again"
    }, error => {
      console.log(error);
      this.errors = error.errors;
    });
  } 

}
