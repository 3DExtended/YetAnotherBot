import { Component, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { forkJoin } from 'rxjs';
import { BotStatusService } from 'src/app/services/bot-status.service';
import { ToggleSwitchComponent } from '../toggle-switch/toggle-switch.component';

@Component({
  selector: 'app-bot-status-indicator',
  templateUrl: './bot-status-indicator.component.html',
  styleUrls: ['./bot-status-indicator.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class BotStatusIndicatorComponent implements OnInit, OnDestroy {
  public botIsRunning = false;

  @ViewChild(ToggleSwitchComponent) toggle: ToggleSwitchComponent | undefined;

  private setTimeoutHandler: any = null;
  constructor(private readonly _botStatusService: BotStatusService) { }

  ngOnInit(): void {
    this.reloadBotStatusForEver(1000);
  }

  ngOnDestroy(): void {
    if (this.setTimeoutHandler) {
      clearTimeout(this.setTimeoutHandler);
      this.setTimeoutHandler = null;
    }
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
    this.setTimeoutHandler = setTimeout(() => {
      this.updateBotStatus();
      this.reloadBotStatusForEver(refreshRate);
    }, refreshRate);
  }

  private updateBotStatus() {
    const botStatusLoader = this._botStatusService.IsBotRunning();
    forkJoin([botStatusLoader]).subscribe(res => {
      this.botIsRunning = res[0].data;
      if (this.toggle) {
        this.toggle.value = this.botIsRunning;
      }
    });
  }
}
