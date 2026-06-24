import { AsyncPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { catchError, map, of, startWith } from 'rxjs';
import { environment } from '../environments/environment';

interface ApiHealthResponse {
  status: string;
  service: string;
  timestamp: string;
}

@Component({
  selector: 'app-root',
  imports: [AsyncPipe],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  private readonly http = inject(HttpClient);

  protected readonly apiBaseUrl = environment.apiBaseUrl;
  protected readonly healthStatus$ = this.http.get<ApiHealthResponse>(`${this.apiBaseUrl}/health`).pipe(
    map((response) => `${response.service} is ${response.status}`),
    startWith('Checking API health...'),
    catchError(() => of('API health check unavailable. Start the ASP.NET Core API and refresh.'))
  );
}
