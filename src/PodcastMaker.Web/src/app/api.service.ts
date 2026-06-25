import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CreateEpisodeRequest {
  topic: string;
  style: string;
  lengthMinutes: number;
  speakers: any[];
}

export interface CreateEpisodeResponse {
  id: string;
  message: string;
}

export interface Episode {
  id: string;
  title: string;
  topic: string;
  style: string;
  lengthMinutes: number;
  createdAt: string;
  status: string;
}

export interface GenerationJob {
  status: string;
  progressPercent: number;
  currentMessage: string;
  errorDetails?: string;
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
  estimatedDurationSeconds: number;
  talkingPoints: string[];
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

  constructor(private http: HttpClient) { }

  getEpisodes(): Observable<Episode[]> {
    return this.http.get<Episode[]>(`${this.baseUrl}/episodes`);
  }

  getEpisode(id: string): Observable<EpisodeDetailsResponse> {
    return this.http.get<EpisodeDetailsResponse>(`${this.baseUrl}/episodes/${id}`);
  }

  createEpisode(request: CreateEpisodeRequest): Observable<CreateEpisodeResponse> {
    return this.http.post<CreateEpisodeResponse>(`${this.baseUrl}/episodes`, request);
  }

  updateSegments(id: string, segments: any[]): Observable<any> {
    return this.http.put(`${this.baseUrl}/episodes/${id}/segments`, segments);
  }

  generateTranscript(id: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/episodes/${id}/generate-transcript`, {});
  }

  regenerateSegment(id: string, segmentId: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/episodes/${id}/segments/${segmentId}/regenerate`, {});
  }

  exportTranscript(id: string, format: string): Observable<ExportTranscriptResponse> {
    return this.http.get<ExportTranscriptResponse>(`${this.baseUrl}/episodes/${id}/export/transcript?format=${format}`);
  }

  getGlobalSettings(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/settings`);
  }

  updateGlobalSettings(settings: any): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/settings`, settings);
  }
}
