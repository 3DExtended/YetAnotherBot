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
import { CustomDialogComponent } from 'src/stories/components/custom-dialog/custom-dialog.component';
import { DropdownMenuComponent } from 'src/stories/components/dropdown-menu/dropdown-menu.component';
import { InputFieldComponent } from 'src/stories/components/input-field/input-field.component';
import { LineGrpahComponent } from 'src/stories/components/line-graph/line-graph.component';
import { LogoComponent } from 'src/stories/components/logo/logo.component';
import { NavbarComponent } from 'src/stories/components/navbar/navbar.component';
import { PipelineElementComponent } from 'src/stories/components/pipeline-element/pipeline-element.component';
import { TableComponent } from 'src/stories/components/table/table.component';
import { ToggleSwitchComponent } from 'src/stories/components/toggle-switch/toggle-switch.component';
import { WizardStepIndicatorComponent } from 'src/stories/components/wizard-step-indicator/wizard-step-indicator.component';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DashboardPageComponent } from './pages/dashboard-page/dashboard-page.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { PipelinePageComponent } from './pages/pipeline-page/pipeline-page.component';
import { RegisterPageComponent } from './pages/register-page/register-page.component';

export function getBaseUrl(): string {

  if (window.location.origin.includes("4200")) {
    return "https://localhost:5001/"
  }

  return window.location.origin + "/";
}

@NgModule({
  declarations: [
    AppComponent,
    BubbleWithIconComponent,
    LineGrpahComponent,
    InputFieldComponent,
    LoginPageComponent,
    ButtonComponent,
    DashboardPageComponent,
    PipelinePageComponent,
    LogoComponent,
    NavbarComponent,
    BotStatusIndicatorComponent,
    ToggleSwitchComponent,
    WizardStepIndicatorComponent,
    TableComponent,
    PipelineElementComponent,
    RegisterPageComponent,
    DropdownMenuComponent,
    CustomDialogComponent
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
  providers: [
    {
      provide: 'API_BASE_URL',
      useFactory: getBaseUrl
    }],
  bootstrap: [AppComponent]
})
export class AppModule { }
