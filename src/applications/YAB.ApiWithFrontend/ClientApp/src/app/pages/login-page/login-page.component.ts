import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { LoginService } from 'src/app/services/login.service';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.css']
})
export class LoginPageComponent implements OnInit {
  public password: string | null = null;
  constructor(private readonly _loginService: LoginService,
    private readonly _activatedRoute: ActivatedRoute,
    private readonly _router: Router
  ) { }

  ngOnInit(): void {
    const IsRegistrationCompletedLoader = this._loginService.IsRegistrationCompleted();
    forkJoin([IsRegistrationCompletedLoader]).subscribe(async (res) => {
      if (res.some(r => !r.successful)) {
        return;
      } else {
        if (res[0].data === false) {
          await this._router.navigateByUrl('/register');
        }
      }
    });
  }

  public async login() {
    console.log('login clicked');
    await this._loginService.Login(this.password ?? '').toPromise()
      .then(async r => {
        console.log(r);
        if (!r.successful && r.statusCode === 404) {
          // some options were not registered correctly. start registering
          await this._router.navigateByUrl('/register');
          return;
        } else if (!r.successful) {
          return;
        }

        // open the status page of the frontend
        await this._router.navigateByUrl('/dashboard');
      });
  }

  /**
   * - click on login should call a login controller (use env. file for storing the url)
   * - we want dont need an access token for now (since you ll be hosting this yourself at first)
   *
   */
}
