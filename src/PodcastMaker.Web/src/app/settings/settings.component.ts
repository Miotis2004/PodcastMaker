import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ApiService } from '../api.service';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatCardModule,
    MatFormFieldModule, MatInputModule, MatButtonModule, MatIconModule
  ],
  template: `
    <div class="settings-container">
      <h2>Global Settings</h2>

      <mat-card>
        <mat-card-header>
          <mat-card-title>Default Speakers</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <form [formGroup]="settingsForm" (ngSubmit)="saveSettings()">
            <div formArrayName="defaultSpeakers">
              <div *ngFor="let speaker of defaultSpeakers.controls; let i=index" [formGroupName]="i" class="speaker-row">
                <mat-form-field appearance="outline">
                  <mat-label>Name</mat-label>
                  <input matInput formControlName="name" required>
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>Role</mat-label>
                  <input matInput formControlName="role" required>
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>Personality</mat-label>
                  <input matInput formControlName="personality" required>
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>Speaking Style</mat-label>
                  <input matInput formControlName="speakingStyle" required>
                </mat-form-field>
                <button mat-icon-button color="warn" type="button" (click)="removeSpeaker(i)" [disabled]="defaultSpeakers.length <= 1">
                  <mat-icon>delete</mat-icon>
                </button>
              </div>
            </div>

            <div class="actions">
              <button mat-button type="button" (click)="addSpeaker()">
                <mat-icon>add</mat-icon> Add Speaker
              </button>
              <button mat-raised-button color="primary" type="submit" [disabled]="settingsForm.invalid || saving">
                {{ saving ? 'Saving...' : 'Save Settings' }}
              </button>
            </div>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .settings-container { max-width: 1000px; margin: 0 auto; padding: 2rem 0; }
    .speaker-row { display: flex; gap: 1rem; align-items: center; margin-bottom: 1rem; }
    .speaker-row mat-form-field { flex: 1; }
    .actions { display: flex; justify-content: space-between; margin-top: 1rem; }
  `]
})
export class SettingsComponent implements OnInit {
  settingsForm: FormGroup;
  saving = false;

  constructor(private fb: FormBuilder, private api: ApiService) {
    this.settingsForm = this.fb.group({
      defaultSpeakers: this.fb.array([])
    });
  }

  ngOnInit() {
    this.loadSettings();
  }

  get defaultSpeakers() {
    return this.settingsForm.get('defaultSpeakers') as FormArray;
  }

  addSpeaker(speaker?: any) {
    this.defaultSpeakers.push(this.fb.group({
      name: [speaker?.name || '', Validators.required],
      role: [speaker?.role || '', Validators.required],
      personality: [speaker?.personality || '', Validators.required],
      speakingStyle: [speaker?.speakingStyle || '', Validators.required]
    }));
  }

  removeSpeaker(index: number) {
    this.defaultSpeakers.removeAt(index);
  }

  loadSettings() {
    this.api.getGlobalSettings().subscribe({
      next: (settings: any) => {
        this.defaultSpeakers.clear();
        if (settings.defaultSpeakers && settings.defaultSpeakers.length > 0) {
          settings.defaultSpeakers.forEach((s: any) => this.addSpeaker(s));
        } else {
          this.addSpeaker({ name: 'Host', role: 'Host', personality: 'Friendly', speakingStyle: 'Casual' });
        }
      },
      error: (err) => console.error('Failed to load settings', err)
    });
  }

  saveSettings() {
    if (this.settingsForm.valid) {
      this.saving = true;
      this.api.updateGlobalSettings(this.settingsForm.value).subscribe({
        next: () => {
          this.saving = false;
        },
        error: (err) => {
          console.error('Failed to save settings', err);
          this.saving = false;
        }
      });
    }
  }
}
