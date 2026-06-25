import { Routes } from '@angular/router';
import { SetupFormComponent } from './setup-form/setup-form';
import { ProjectListComponent } from './project-list/project-list';
import { EpisodeViewerComponent } from './episode-viewer/episode-viewer';
import { SettingsComponent } from './settings/settings.component';

export const routes: Routes = [
  { path: '', redirectTo: 'projects', pathMatch: 'full' },
  { path: 'projects', component: ProjectListComponent },
  { path: 'new', component: SetupFormComponent },
  { path: 'episode/:id', component: EpisodeViewerComponent },
  { path: 'settings', component: SettingsComponent }
];
