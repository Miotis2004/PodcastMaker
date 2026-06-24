import { Component } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, MatToolbarModule, MatButtonModule, RouterLink],
  template: `
    <mat-toolbar color="primary">
      <span>PodcastMaker Studio</span>
      <span class="spacer"></span>
      <button mat-button routerLink="/projects">Library</button>
      <button mat-raised-button color="accent" routerLink="/new">New Episode</button>
    </mat-toolbar>
    <main class="content-wrapper">
      <router-outlet></router-outlet>
    </main>
  `,
  styles: [`
    .spacer { flex: 1 1 auto; }
    .content-wrapper { padding: 2rem; max-width: 1200px; margin: 0 auto; }
  `]
})
export class AppComponent {}
