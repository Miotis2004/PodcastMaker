import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { Router } from '@angular/router';
import { ApiService } from '../api.service';

@Component({
  selector: 'app-setup-form',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatFormFieldModule,
    MatInputModule, MatSelectModule, MatButtonModule, MatCardModule
  ],
  template: `
    <div class="setup-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Create New Episode</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <form [formGroup]="setupForm" (ngSubmit)="onSubmit()" class="setup-form">

            <mat-form-field appearance="outline">
              <mat-label>Topic</mat-label>
              <textarea matInput formControlName="topic" placeholder="e.g. The history of mechanical keyboards" rows="3"></textarea>
              <mat-error *ngIf="setupForm.get('topic')?.hasError('required')">Topic is required</mat-error>
            </mat-form-field>

            <div class="row">
              <mat-form-field appearance="outline">
                <mat-label>Length (Minutes)</mat-label>
                <mat-select formControlName="lengthMinutes">
                  <mat-option [value]="5">5 Minutes</mat-option>
                  <mat-option [value]="15">15 Minutes</mat-option>
                </mat-select>
              </mat-form-field>

              <mat-form-field appearance="outline">
                <mat-label>Style</mat-label>
                <mat-select formControlName="style">
                  <mat-option value="Casual Podcast">Casual Podcast</mat-option>
                  <mat-option value="Debate">Debate</mat-option>
                  <mat-option value="Educational">Educational</mat-option>
                </mat-select>
              </mat-form-field>
            </div>

            <div class="row">
              <mat-form-field appearance="outline">
                <mat-label>Host Name</mat-label>
                <input matInput formControlName="hostName">
              </mat-form-field>

              <mat-form-field appearance="outline">
                <mat-label>Guest Name</mat-label>
                <input matInput formControlName="guestName">
              </mat-form-field>
            </div>

            <div class="actions">
              <button mat-button type="button" routerLink="/projects">Cancel</button>
              <button mat-raised-button color="primary" type="submit" [disabled]="setupForm.invalid || submitting">
                {{ submitting ? 'Starting...' : 'Generate Outline' }}
              </button>
            </div>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .setup-container { max-width: 800px; margin: 0 auto; }
    .setup-form { display: flex; flex-direction: column; margin-top: 1rem; }
    .row { display: flex; gap: 1rem; }
    .row mat-form-field { flex: 1; }
    .actions { display: flex; justify-content: flex-end; gap: 1rem; margin-top: 1rem; }
  `]
})
export class SetupFormComponent {
  setupForm: FormGroup;
  submitting = false;

  constructor(private fb: FormBuilder, private api: ApiService, private router: Router) {
    this.setupForm = this.fb.group({
      topic: ['', Validators.required],
      lengthMinutes: [5, Validators.required],
      style: ['Casual Podcast', Validators.required],
      hostName: ['Host', Validators.required],
      guestName: ['Expert', Validators.required]
    });
  }

  onSubmit() {
    if (this.setupForm.valid) {
      this.submitting = true;
      this.api.createEpisode(this.setupForm.value).subscribe({
        next: (response) => {
          this.router.navigate(['/episode', response.id]);
        },
        error: (err) => {
          console.error(err);
          this.submitting = false;
        }
      });
    }
  }
}
