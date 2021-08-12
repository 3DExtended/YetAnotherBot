import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RegisterService } from 'src/app/services/register.service';

@Component({
  selector: 'app-register-page',
  templateUrl: './register-page.component.html',
  styleUrls: ['./register-page.component.css']
})
export class RegisterPageComponent implements OnInit {

  constructor(private readonly _registerService: RegisterService,
    private readonly _activatedRoute: ActivatedRoute,
    private readonly _router: Router) { }

  ngOnInit(): void {
  }

}
