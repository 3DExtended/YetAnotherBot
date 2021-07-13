import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { NgxGraphModule } from '@swimlane/ngx-graph';
import { ChartsModule } from 'ng2-charts';
import { BotStatusIndicatorComponent } from 'src/stories/components/bot-status-indicator/bot-status-indicator.component';
import { BubbleWithIconComponent } from 'src/stories/components/bubble-with-icon/bubble-with-icon.component';
import { ButtonComponent } from 'src/stories/components/button/button.component';
import { InputFieldComponent } from 'src/stories/components/input-field/input-field.component';
import { LineGrpahComponent } from 'src/stories/components/line-graph/line-graph.component';
import { LogoComponent } from 'src/stories/components/logo/logo.component';
import { NavbarComponent } from 'src/stories/components/navbar/navbar.component';
import { ToggleSwitchComponent } from 'src/stories/components/toggle-switch/toggle-switch.component';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DashboardPageComponent } from './pages/dashboard-page/dashboard-page.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';


@NgModule({
  declarations: [
    AppComponent,
    BubbleWithIconComponent,
    LineGrpahComponent,
    InputFieldComponent,
    LoginPageComponent,
    ButtonComponent,
    DashboardPageComponent,
    LogoComponent,
    NavbarComponent,
    BotStatusIndicatorComponent,
    ToggleSwitchComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ChartsModule,
    NgxGraphModule,
    FormsModule,
    CommonModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
