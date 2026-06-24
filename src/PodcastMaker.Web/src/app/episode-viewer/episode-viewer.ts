import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { ApiService, EpisodeDetailsResponse } from '../api.service';
import { Subscription, interval } from 'rxjs';

@Component({
  selector: 'app-episode-viewer',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatProgressBarModule, MatIconModule, MatMenuModule],
  template: `
    <div class="viewer-container" *ngIf="details">
      <div class="header-actions">
        <div>
          <h2>{{ details.episode.title }}</h2>
          <p class="subtitle">Topic: {{ details.episode.topic }} | Style: {{ details.episode.style }}</p>
        </div>
        <div>
          <button mat-raised-button color="primary"
                  *ngIf="details.episode.status === 'OutlineGenerated'"
                  (click)="startTranscriptGeneration()">
            Generate Transcript
          </button>

          <button mat-button [matMenuTriggerFor]="exportMenu"
                  *ngIf="details.episode.status === 'TranscriptGenerated'">
            <mat-icon>download</mat-icon> Export
          </button>
          <mat-menu #exportMenu="matMenu">
            <button mat-menu-item (click)="exportTranscript('txt')">Plain Text (.txt)</button>
            <button mat-menu-item (click)="exportTranscript('md')">Markdown (.md)</button>
          </mat-menu>
        </div>
      </div>

      <!-- Job Status -->
      <mat-card class="status-card" *ngIf="details.job.status === 'Running'">
        <mat-card-content>
          <p>{{ details.job.currentMessage }}</p>
          <mat-progress-bar mode="determinate" [value]="details.job.progressPercent"></mat-progress-bar>
        </mat-card-content>
      </mat-card>

      <mat-card class="status-card error" *ngIf="details.job.status === 'Failed'">
        <mat-card-content>
          <mat-icon color="warn">error</mat-icon>
          <p>Error: {{ details.job.errorDetails }}</p>
        </mat-card-content>
      </mat-card>

      <!-- Transcript Viewer -->
      <div class="segments" *ngIf="details.segments && details.segments.length > 0">
        <div class="segment-block" *ngFor="let seg of details.segments">
          <h3>{{ seg.segment.title }}</h3>
          <p class="purpose">Goal: {{ seg.segment.purpose }}</p>

          <div class="dialogue" *ngIf="seg.dialogueLines && seg.dialogueLines.length > 0">
            <div class="line" *ngFor="let line of seg.dialogueLines">
              <strong class="speaker" [ngClass]="getSpeakerClass(line.speakerName)">
                {{ line.speakerName }}:
              </strong>
              <span class="text">{{ line.text }}</span>
            </div>
          </div>

          <div class="empty-transcript" *ngIf="!seg.dialogueLines || seg.dialogueLines.length === 0">
            <p>No transcript generated for this segment yet.</p>
          </div>
        </div>
      </div>

    </div>
  `,
  styles: [`
    .viewer-container { max-width: 1000px; margin: 0 auto; }
    .header-actions { display: flex; justify-content: space-between; align-items: center; margin-bottom: 2rem; border-bottom: 1px solid #eee; padding-bottom: 1rem; }
    .header-actions h2 { margin: 0; }
    .subtitle { color: #666; margin: 0; font-size: 0.9rem; }
    .status-card { margin-bottom: 2rem; }
    .status-card.error { border-left: 4px solid red; }
    .segment-block { margin-bottom: 3rem; }
    .segment-block h3 { margin-bottom: 0.5rem; border-bottom: 1px solid #eee; padding-bottom: 0.5rem; }
    .purpose { color: #666; font-style: italic; margin-bottom: 1.5rem; font-size: 0.9rem; }
    .dialogue .line { margin-bottom: 1rem; line-height: 1.5; }
    .speaker { font-weight: bold; margin-right: 0.5rem; }
    .speaker.host { color: #1976d2; }
    .speaker.guest { color: #388e3c; }
    .empty-transcript { background: #f9f9f9; padding: 1rem; border-radius: 4px; text-align: center; color: #888; }
  `]
})
export class EpisodeViewerComponent implements OnInit, OnDestroy {
  details: EpisodeDetailsResponse | null = null;
  episodeId: string = '';
  pollingSub: Subscription | null = null;

  constructor(
    private route: ActivatedRoute,
    private api: ApiService
  ) {}

  ngOnInit() {
    this.episodeId = this.route.snapshot.paramMap.get('id') || '';
    if (this.episodeId) {
      this.loadEpisode();
      this.startPolling();
    }
  }

  ngOnDestroy() {
    this.stopPolling();
  }

  loadEpisode() {
    this.api.getEpisode(this.episodeId).subscribe({
      next: (data) => {
        this.details = data;

        // Stop polling if completed or failed
        if (data.job.status === 'Completed' || data.job.status === 'Failed' || data.job.status === 'Draft') {
          // If status is generating, keep polling. Wait, we want to stop if it's done.
          if (data.episode.status !== 'GeneratingOutline' && data.episode.status !== 'GeneratingTranscript') {
             this.stopPolling();
          }
        }
      },
      error: (err) => console.error(err)
    });
  }

  startPolling() {
    if (!this.pollingSub) {
      this.pollingSub = interval(2000).subscribe(() => {
        this.loadEpisode();
      });
    }
  }

  stopPolling() {
    if (this.pollingSub) {
      this.pollingSub.unsubscribe();
      this.pollingSub = null;
    }
  }

  startTranscriptGeneration() {
    this.api.generateTranscript(this.episodeId).subscribe({
      next: () => {
        this.startPolling();
      },
      error: (err) => console.error(err)
    });
  }

  exportTranscript(format: string) {
    this.api.exportTranscript(this.episodeId, format).subscribe({
      next: (res) => {
        const blob = new Blob([res.content], { type: 'text/plain' });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = res.filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      },
      error: (err) => console.error(err)
    });
  }

  getSpeakerClass(name: string): string {
    // A simple heuristic for coloring
    if (name.toLowerCase().includes('host')) return 'host';
    if (name.toLowerCase().includes('guest')) return 'guest';
    return '';
  }
}
