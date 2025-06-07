import { Component } from '@angular/core';
import { AuthService } from '../core/auth.service';
import {Router, RouterLink, RouterOutlet} from '@angular/router';
import {MatToolbar} from '@angular/material/toolbar';
import {MatSidenav, MatSidenavContainer, MatSidenavModule} from '@angular/material/sidenav';
import {MatButton} from '@angular/material/button';
import {CommonModule} from '@angular/common';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  templateUrl: './main-layout.component.html',
  imports: [
    CommonModule,
    MatToolbar,
    MatSidenavContainer,
    MatSidenavModule,
    RouterOutlet,
    MatSidenav,
    MatButton,
    RouterLink
  ],
  styleUrls: ['./main-layout.component.css']
})
export class MainLayoutComponent {
  constructor(public auth: AuthService, private router: Router) {}

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
