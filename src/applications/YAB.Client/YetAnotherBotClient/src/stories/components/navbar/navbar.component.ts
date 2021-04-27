import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class NavbarComponent implements OnInit {

  @Input() loggedIn: boolean = false;

  private navbarElements = [
    {
      href: "#",
      titleKey: 'Pricing',
      showOn: () => { return !this.loggedIn; }
    },
    {
      href: "#",
      titleKey: 'Features',
      showOn: () => { return !this.loggedIn; }
    },
    {
      href: "#",
      titleKey: 'Dashboard',
      showOn: () => { return this.loggedIn; }
    },
    {
      href: "#",
      titleKey: 'Pipelines',
      showOn: () => { return this.loggedIn; }
    },
    {
      href: "#",
      titleKey: 'Extensions',
      showOn: () => { return this.loggedIn; }
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
