import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { Router } from '@angular/router';
import { ApiService, Episode } from '../api.service';

@Component({
  selector: 'app-project-list',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatChipsModule],
  template: `
    <h2>Recent Projects</h2>
    <div *ngIf="episodes.length === 0" class="empty-state">
      <p>No episodes yet. Create one!</p>
      <button mat-raised-button color="primary" (click)="newEpisode()">Create Episode</button>
    </div>

    <div class="card-grid">
      <mat-card *ngFor="let ep of episodes" class="episode-card">
        <mat-card-header>
          <mat-card-title>{{ ep.title }}</mat-card-title>
          <mat-card-subtitle>{{ ep.topic }}</mat-card-subtitle>
        </mat-card-header>
        <mat-card-content>
          <p>Style: {{ ep.style }}</p>
          <mat-chip-set>
            <mat-chip>{{ ep.status }}</mat-chip>
            <mat-chip>{{ ep.lengthMinutes }}m</mat-chip>
          </mat-chip-set>
        </mat-card-content>
        <mat-card-actions>
          <button mat-button (click)="openEpisode(ep.id)">Open</button>
        </mat-card-actions>
      </mat-card>
    </div>
  `,
  styles: [`
    .empty-state { text-align: center; padding: 4rem; color: #666; }
    .card-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 1.5rem; }
    .episode-card { display: flex; flex-direction: column; }
    .episode-card mat-card-content { flex-grow: 1; margin-top: 1rem; }
  `]
})
export class ProjectListComponent implements OnInit {
  episodes: Episode[] = [];

  constructor(private api: ApiService, private router: Router) {}

  ngOnInit() {
    this.api.getEpisodes().subscribe(data => {
      this.episodes = data;
    });
  }

  newEpisode() {
    this.router.navigate(['/new']);
  }

  openEpisode(id: string) {
    this.router.navigate(['/episode', id]);
  }
}
