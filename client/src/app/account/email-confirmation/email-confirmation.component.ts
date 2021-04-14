import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../account.service';

@Component({
  selector: 'app-email-confirmation',
  templateUrl: './email-confirmation.component.html',
  styleUrls: ['./email-confirmation.component.scss']
})
export class EmailConfirmationComponent implements OnInit {

  errors: string[];
  isSuccess: boolean;

  constructor(private accService: AccountService,  private activatedRoute: ActivatedRoute, 
    private router: Router, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.confirmEmail();
  }

  confirmEmail() {
    this.isSuccess = false;

    const token = this.activatedRoute.snapshot.queryParams['token'];
    const email = this.activatedRoute.snapshot.queryParams['email'];

    console.log(token);

    this.accService.confirmEmail(token, email).subscribe(() => {
      console.log("Email confirmed");
      this.isSuccess = true;
      this.toastr.success('Email confirmed');
      this.router.navigateByUrl('/shop');
    },error => {
      console.log(error.Message);
      this.isSuccess = false;
      this.errors = error.errors;
      this.toastr.error('Email confirmation failed');
    })
  }

}
