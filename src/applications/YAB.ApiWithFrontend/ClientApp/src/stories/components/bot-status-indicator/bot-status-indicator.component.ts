import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { Subject, timer } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';
import { BotStatusService } from 'src/app/services/bot-status.service';
import { ToggleSwitchComponent } from '../toggle-switch/toggle-switch.component';

@Component({
  selector: 'app-bot-status-indicator',
  templateUrl: './bot-status-indicator.component.html',
  styleUrls: ['./bot-status-indicator.component.css'],
  // encapsulation: ViewEncapsulation.None
})
export class BotStatusIndicatorComponent implements OnInit {
  public botIsRunning = false;

  @ViewChild(ToggleSwitchComponent) toggle: ToggleSwitchComponent | undefined;

  private errorRefreshingBotStatus: Subject<boolean> = new Subject<boolean>();

  constructor(private readonly _botStatusService: BotStatusService,
    private readonly _router: Router) { }

  ngOnInit(): void {
    this.reloadBotStatusForEver(1000);
  }

  public toggleTheBotState(newBotStatus: boolean) {
    if (newBotStatus === this.botIsRunning) {
      return;
    }

    if (newBotStatus) {
      this._botStatusService.StartBot()
        .subscribe(() => {
          this.botIsRunning = true;
          if (this.toggle) {
            this.toggle.value = true;
          }
        });
    } else {
      this._botStatusService.StopBot()
        .subscribe(() => {
          this.botIsRunning = false;
          if (this.toggle) {
            this.toggle.value = false;
          }
        });
    }
  }

  private reloadBotStatusForEver(refreshRate: number) {
    timer(0, refreshRate).pipe(
      takeUntil(this.errorRefreshingBotStatus),
      map(_ => {
        return this._botStatusService.IsBotRunning();
      })
    )
      .subscribe({
        next: status => {
          status.subscribe(async (res) => {
            if (!res.successful) {
              this.errorRefreshingBotStatus.next(false);

              await this._router.navigateByUrl('/login');
              return;
            }
            this.botIsRunning = res.data;
            if (this.toggle) {
              this.toggle.value = this.botIsRunning;
            }
          });
        }
      });
  }
}
