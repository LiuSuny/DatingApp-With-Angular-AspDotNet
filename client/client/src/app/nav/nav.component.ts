import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { Route, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit{
  
  model: any = {}
  //loggedIn = false
  //currentUser$: Observable<User>;

  constructor(public accountService: AccountService, 
    private router: Router, private toastr: ToastrService){}
  
  ngOnInit(){
    
  }

  login(){
   this.accountService.login(this.model).subscribe(response =>{
    this.router.navigateByUrl('/members');
   });
  };

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

  // getCurrentUser(){
  //   this.accountService.currentUser$.subscribe(user =>{
  //     this.loggedIn = !!user
  //   }, error =>{
  //     console.log(error);
  //   });
  // }

}
