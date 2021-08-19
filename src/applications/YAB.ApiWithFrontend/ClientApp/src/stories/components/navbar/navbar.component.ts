import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
  // encapsulation: ViewEncapsulation.None
})
export class NavbarComponent implements OnInit {

  private navbarElements = [
    {
      href: "#/dashboard",
      titleKey: 'Dashboard',
      showOn: () => { return true; }
    },
    {
      href: "#/pipelines",
      titleKey: 'Pipelines',
      showOn: () => { return true; }
    },
    {
      href: "#/extensions",
      titleKey: 'Extensions',
      showOn: () => { return true; }
    },
  ];

  public isMobileMenuShowing: boolean = false;

  constructor() { }

  ngOnInit(): void {
  }

  public getNavbarItemsToShow() {
    return this.navbarElements.filter(e => e.showOn());
  }

  public onMobileMenuButtonClicked(event: MouseEvent) {
    this.isMobileMenuShowing = !this.isMobileMenuShowing;
  }
}
