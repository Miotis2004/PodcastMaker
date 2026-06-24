import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Episode {
  id: string;
  title: string;
  topic: string;
  style: string;
  lengthMinutes: number;
  status: string;
  createdAt: string;
}

export interface GenerationJob {
  status: string;
  progressPercent: number;
  currentMessage: string;
  errorDetails: string;
  startedAt: string;
  completedAt: string;
}

export interface DialogueLine {
  id: string;
  segmentId: string;
  speakerName: string;
  text: string;
  sortOrder: number;
}

export interface Segment {
  id: string;
  episodeId: string;
  title: string;
  sortOrder: number;
  purpose: string;
  transcript: string;
}

export interface SegmentResponse {
  segment: Segment;
  dialogueLines: DialogueLine[];
}

export interface EpisodeDetailsResponse {
  episode: Episode;
  job: GenerationJob;
  speakers: any[];
  segments: SegmentResponse[];
}

export interface CreateEpisodeRequest {
  topic: string;
  style: string;
  lengthMinutes: number;
  hostName: string;
  guestName: string;
}

export interface CreateEpisodeResponse {
  id: string;
  message: string;
}

export interface ExportTranscriptResponse {
  format: string;
  content: string;
  filename: string;
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = 'http://localhost:5000/api';

  constructor(private http: HttpClient) {}

  getEpisodes(): Observable<Episode[]> {
    return this.http.get<Episode[]>(`${this.baseUrl}/episodes`);
  }

  getEpisode(id: string): Observable<EpisodeDetailsResponse> {
    return this.http.get<EpisodeDetailsResponse>(`${this.baseUrl}/episodes/${id}`);
  }

  createEpisode(request: CreateEpisodeRequest): Observable<CreateEpisodeResponse> {
    return this.http.post<CreateEpisodeResponse>(`${this.baseUrl}/episodes`, request);
  }

  generateTranscript(id: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/episodes/${id}/generate-transcript`, {});
  }

  exportTranscript(id: string, format: string): Observable<ExportTranscriptResponse> {
    return this.http.get<ExportTranscriptResponse>(`${this.baseUrl}/episodes/${id}/export/transcript?format=${format}`);
  }
}
