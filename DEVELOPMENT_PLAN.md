# PodcastMaker Development Plan

## Purpose

This plan turns the README vision for PodcastMaker/DialogCast into an implementation roadmap. The target product is a local-first AI podcast studio that accepts a topic, generates a structured two-speaker podcast transcript with Ollama, optionally converts it to speech with local TTS, and exports reusable project artifacts.

## Guiding Principles

1. **Transcript quality first**: Do not start audio polish until outline and transcript generation are reliable.
2. **Local-first by default**: Ollama, project storage, TTS, and exports should work without cloud services.
3. **Long jobs are observable and recoverable**: Generation should expose progress, support cancellation, and preserve partial output.
4. **Structured data over raw text**: Persist episodes, speakers, segments, dialogue lines, prompts, and job states as typed records.
5. **Replaceable engines**: LLM, TTS, audio, and storage implementations should be hidden behind interfaces.
6. **Small vertical slices**: Every milestone should produce a runnable application with one or more user-visible improvements.

## Phase 0: Repository and Architecture Foundation

### Goals

Create the initial solution layout, development tooling, and architectural boundaries before implementing user features.

### Steps

1. **Create solution structure**
   - Create an ASP.NET Core Web API project under `src/PodcastMaker.Api`.
   - Create a domain/application class library under `src/PodcastMaker.Core`.
   - Create an infrastructure class library under `src/PodcastMaker.Infrastructure`.
   - Create a test project under `tests/PodcastMaker.Tests`.
   - Create an Angular app under `src/PodcastMaker.Web`.
2. **Define backend layering**
   - `Core`: domain models, DTOs, service interfaces, validation abstractions.
   - `Infrastructure`: Ollama client, file storage, SQLite persistence, TTS/audio adapters.
   - `Api`: controllers/minimal endpoints, SignalR hubs, dependency injection, configuration.
3. **Add common developer commands**
   - Add a root `README` section or script file documenting build, test, run, and formatting commands.
   - Add `.editorconfig` for C# and TypeScript formatting consistency.
4. **Set up configuration**
   - Define settings for Ollama URL, default model, project storage path, SQLite path, TTS engine, FFmpeg path, and allowed export types.
   - Provide safe development defaults in `appsettings.Development.json`.
5. **Add basic health checks**
   - API health endpoint.
   - Ollama connectivity check endpoint.
   - Frontend environment configuration for API base URL.

### Deliverables

- Runnable empty Angular shell.
- Runnable ASP.NET Core API.
- Test project wired into the solution.
- Basic health/configuration endpoints.

### Acceptance Criteria

- `dotnet build` succeeds.
- `dotnet test` succeeds with initial placeholder tests.
- Angular app starts and can call the API health endpoint.

## Phase 1: Transcript MVP

### Goals

Implement the minimum first prototype from the README: topic input, length selection, outline generation, transcript generation, transcript display, and transcript export.

### Backend Steps

1. **Create core domain models**
   - `Episode` with ID, title, topic, style, length, status, timestamps.
   - `Speaker` with name, role, personality, speaking style, optional voice ID.
   - `Segment` with title, purpose, sort order, estimated duration, talking points, transcript.
   - `DialogueLine` with speaker name, text, segment ID, sort order, optional audio path.
   - `GenerationJob` with status, progress, timestamps, error details, cancellation marker.
2. **Create request/response contracts**
   - Create episode request.
   - Generate outline request/response.
   - Generate transcript request/response.
   - Export transcript response.
   - Error response model with machine-readable code and human-readable message.
3. **Implement project storage service**
   - Create one folder per episode.
   - Persist `metadata.json`, `outline.json`, `transcript.json`, and `transcript.txt`.
   - Use atomic writes where possible to prevent corrupt files.
4. **Implement Ollama service**
   - Support model listing/availability check.
   - Support completion/chat generation.
   - Add timeout configuration and meaningful errors for Ollama not running or model missing.
5. **Implement prompt templates**
   - Topic analysis prompt.
   - Outline prompt that returns strict JSON.
   - Segment dialogue prompt.
   - Cleanup pass prompt.
6. **Implement outline generation**
   - Generate episode title, description, and 5-8 segments.
   - Validate LLM JSON and retry once with a repair prompt if invalid.
   - Persist outline and update episode status to `OutlineGenerated`.
7. **Implement transcript generation**
   - Generate each segment independently.
   - Include previous segment summary to maintain continuity.
   - Parse dialogue lines into structured records.
   - Persist partial output after each segment.
   - Update status through `GeneratingTranscript` and `TranscriptGenerated`.
8. **Implement transcript formatting/export**
   - Plain text export.
   - Markdown export.
   - Copy-friendly display format.
9. **Expose API endpoints**
   - `POST /api/episodes`
   - `POST /api/episodes/{episodeId}/outline`
   - `POST /api/episodes/{episodeId}/generate-transcript`
   - `GET /api/episodes/{episodeId}`
   - `GET /api/episodes/{episodeId}/export/transcript?format=txt|md`
10. **Add tests**
    - Unit tests for prompt rendering.
    - Unit tests for transcript parsing.
    - Unit tests for project storage read/write.
    - Integration test with a fake LLM client.

### Frontend Steps

1. **Create home/setup page**
   - Topic input.
   - Length selector for 5 and 15 minutes.
   - Style selector.
   - Default Host and Expert Guest profiles.
2. **Create generation workflow**
   - Start episode creation.
   - Generate outline.
   - Generate transcript.
   - Show loading/progress states.
   - Show user-friendly errors.
3. **Create transcript viewer**
   - Render speaker-labeled dialogue.
   - Display episode title, topic, style, and segment headings.
   - Add copy transcript button.
   - Add TXT and Markdown export links.
4. **Add basic recent projects view**
   - List locally persisted episodes returned by the API.
   - Reopen a generated transcript.

### Deliverables

- End-to-end transcript generation from the web UI.
- Local episode folders with metadata, outline, and transcript artifacts.
- Exportable TXT and Markdown transcripts.

### Acceptance Criteria

- A user can generate a 5-minute and 15-minute transcript.
- Speaker names remain consistent across all segments.
- Partial transcript output is saved if generation fails mid-run.
- Exported transcript matches the transcript shown in the UI.

## Phase 2: Quality, Editing, and Regeneration

### Goals

Improve podcast structure and give users control before and after generation.

### Steps

1. **Outline editor**
   - Edit segment titles, purposes, durations, and talking points.
   - Add, remove, and reorder segments.
   - Validate total estimated duration against target length.
2. **Speaker profile editor**
   - Edit name, role, personality, and speaking style.
   - Save default speaker presets.
3. **Style and tone controls**
   - Add Educational, Casual Podcast, Debate, Technical Deep Dive, Storytelling, and Comedy modes.
   - Map each style to explicit prompt guidance.
4. **Segment regeneration**
   - Regenerate one segment without replacing the entire transcript.
   - Preserve neighboring segment summaries for continuity.
5. **Rewrite selected passage**
   - Accept selected transcript text and rewrite instructions.
   - Replace only the selected section after user confirmation.
6. **Title, intro, outro, and show notes generation**
   - Generate multiple title options.
   - Generate intro/outro templates.
   - Generate show notes with summary, chapters, and key points.
7. **Continuity and cleanup pass**
   - Add optional cleanup pass after full generation.
   - Produce a before/after preview for user approval.
8. **Testing**
   - Add fake-model tests for regeneration boundaries.
   - Add UI tests for editing and export flows.

### Acceptance Criteria

- Users can edit an outline before transcript generation.
- Users can regenerate a single segment without losing the rest of the episode.
- Different styles produce noticeably different prompt inputs and transcript structures.

## Phase 3: Persistence and Project Library

### Goals

Move from simple file artifacts to a durable project library that supports many episodes.

### Steps

1. **Add SQLite persistence**
   - Use Entity Framework Core or a lightweight repository implementation.
   - Add tables for Episodes, Segments, Speakers, DialogueLines, Voices, AudioFiles, GenerationJobs, and Settings.
2. **Create migration strategy**
   - Add initial schema migration.
   - Preserve compatibility with project folder JSON exports.
3. **Implement project dashboard APIs**
   - Search by title/topic.
   - Sort by created date, updated date, length, and status.
   - Duplicate and delete episodes.
4. **Implement frontend library page**
   - Search, sort, open, duplicate, and delete projects.
   - Show status badges and generated artifact availability.
5. **Resume unfinished generation**
   - Detect partial output.
   - Continue from the next missing segment.
6. **Settings page**
   - Configure Ollama URL, model, output folder, FFmpeg path, and default speaker profiles.

### Acceptance Criteria

- Users can manage dozens of projects.
- Unfinished jobs can be resumed or safely discarded.
- Project library and project folders remain consistent.

## Phase 4: Long-Running Jobs and Live Updates

### Goals

Make generation reliable for longer episodes and provide responsive progress feedback.

### Steps

1. **Add background job queue**
   - Queue outline, transcript, cleanup, audio, and export tasks.
   - Track job state: Draft, Queued, Running, Completed, Failed, Cancelled.
2. **Add cancellation support**
   - Allow users to cancel generation.
   - Persist partial results and cancellation status.
3. **Add SignalR progress hub**
   - Broadcast stage, segment number, percent complete, current message, and errors.
4. **Improve retries and recovery**
   - Retry transient LLM failures.
   - Add JSON repair for malformed outline output.
   - Record prompt and model metadata for debugging.
5. **Frontend progress UI**
   - Show current phase and segment progress.
   - Provide cancel and retry actions.

### Acceptance Criteria

- Transcript generation does not block HTTP requests until completion.
- Browser refresh does not lose job state.
- Users can cancel long-running generation.

## Phase 5: TTS Audio Generation

### Goals

Convert transcripts into understandable two-voice audio using a local TTS engine.

### Steps

1. **Define TTS abstraction**
   - Interface for listing voices, synthesizing text, and checking engine readiness.
   - Piper implementation first.
   - Reserve extension point for Kokoro.
2. **Voice assignment UI**
   - Assign one voice to Host and one voice to Guest.
   - Preview a short voice sample.
3. **Generate line or paragraph WAV files**
   - Split transcript into TTS-safe chunks.
   - Generate per-line or per-paragraph audio under `audio/`.
   - Store audio path on each `DialogueLine`.
4. **Add audio job queue**
   - Process TTS in the background.
   - Resume from already generated clips.
5. **Add audio preview page**
   - Show per-line generation status.
   - Preview individual clips and the assembled episode.
6. **Testing**
   - Unit test chunking and filename generation.
   - Integration test with fake TTS adapter.

### Acceptance Criteria

- Users can generate WAV clips for each speaker.
- Host and Guest use different voices.
- Audio generation can resume after interruption.

## Phase 6: Audio Assembly and Export

### Goals

Produce podcast-ready audio files from generated clips.

### Steps

1. **Add FFmpeg service**
   - Validate FFmpeg availability.
   - Stitch clips in dialogue order.
   - Insert configurable pauses between turns and segments.
2. **Normalize and convert audio**
   - Normalize volume/loudness.
   - Export WAV and MP3.
3. **Add export endpoints**
   - `POST /api/episodes/{episodeId}/generate-audio`
   - `GET /api/episodes/{episodeId}/export/audio?format=mp3|wav`
4. **Add audio metadata**
   - Optional ID3 title, author/show name, description, and AI-generated label.
5. **Frontend audio export UI**
   - Generate, preview, download MP3/WAV.
   - Show errors for missing TTS engine or FFmpeg.

### Acceptance Criteria

- Users can download a final MP3.
- Audio levels are consistent enough for a first release.
- Missing FFmpeg/TTS dependencies produce actionable setup messages.

## Phase 7: Audio Polish

### Goals

Improve realism and production quality.

### Steps

1. Add intro/outro music support with user-provided assets.
2. Add transition stingers between segments.
3. Add room tone and configurable pauses.
4. Add pronunciation dictionary.
5. Add voice speed and pitch controls where supported by the TTS engine.
6. Add chapter marker export.
7. Add automatic loudness target presets for podcast platforms.

### Acceptance Criteria

- Final audio sounds more polished than simple stitched dialogue.
- Users can control production elements without editing files manually.

## Phase 8: Advanced Generation Modes

### Goals

Make show formats feel distinct and support user-provided context.

### Steps

1. Add Debate, Interview, Explainer, Comedy, Documentary, and Storytelling generation pipelines.
2. Add user notes input and document ingestion.
3. Add source-aware outlines and transcript references where documents are provided.
4. Add fact-check/critic pass with configurable model.
5. Add model selection per generation role.
6. Add custom system prompt support with clear warnings.

### Acceptance Criteria

- Each mode changes structure, tone, and speaker behavior meaningfully.
- User-provided notes influence the generated episode.
- Fact-check pass surfaces questionable claims without silently rewriting everything.

## Phase 9: Optional Video Export

### Goals

Create YouTube-ready MP4 outputs.

### Steps

1. Add static background image selection.
2. Add speaker avatars.
3. Generate waveform animation from final audio.
4. Generate captions from dialogue timing or TTS clip timing.
5. Add title card and progress bar.
6. Export MP4 with FFmpeg or a rendering pipeline such as Remotion.

### Acceptance Criteria

- Users can export an MP4 with title, waveform, and captions.
- Captions align reasonably with spoken audio.

## Phase 10: Optional Voice Cloning

### Goals

Support custom voices while enforcing consent and safety constraints.

### Steps

1. Add voice sample upload/import workflow.
2. Add local-only cloned voice storage.
3. Add consent confirmation before creating or using a cloned voice.
4. Add clear warnings against impersonation or fraudulent use.
5. Add voice profile management and deletion.
6. Add generated audio metadata indicating AI-generated content.

### Acceptance Criteria

- Users can create and use custom voice profiles they have permission to use.
- The app clearly discourages impersonation and unsafe voice use.

## Cross-Cutting Implementation Requirements

### Error Handling

Handle and test these scenarios throughout development:

- Ollama is not running.
- The configured model is not installed.
- LLM calls time out.
- LLM returns invalid JSON.
- TTS engine is missing or fails.
- FFmpeg is missing or fails.
- Disk writes fail.
- User cancels a job.
- Browser refreshes during a long-running job.

### Security and Privacy

- No telemetry by default.
- No cloud account required.
- Store projects locally by default.
- Make output folder user-configurable.
- Label generated audio as AI-generated where metadata is supported.
- Do not add impersonation features.

### Testing Strategy

1. **Unit tests**
   - Prompt templates.
   - JSON parsing/repair.
   - Transcript parsing.
   - Dialogue chunking.
   - Export formatting.
2. **Integration tests**
   - API endpoints with fake LLM/TTS adapters.
   - Project storage and SQLite persistence.
   - Job queue state transitions.
3. **Frontend tests**
   - Setup form validation.
   - Generation progress states.
   - Transcript viewer and export controls.
   - Outline editor behavior.
4. **Manual smoke tests**
   - Generate a 5-minute transcript.
   - Generate a 15-minute transcript.
   - Export TXT/Markdown.
   - Generate MP3 after TTS milestone.

### Observability

- Structured backend logs with episode ID and job ID.
- Store prompt/model metadata for failed jobs.
- Surface concise user errors in the UI.
- Keep detailed errors in logs for debugging.

## Suggested Release Milestones

| Milestone | Release Theme | User-Visible Outcome |
| --- | --- | --- |
| 0 | Foundation | App shell and API are runnable. |
| 1 | Transcript MVP | Generate and export 5-15 minute transcripts. |
| 2 | Editing | Edit outlines, speakers, and individual segments. |
| 3 | Library | Save, search, reopen, duplicate, and delete projects. |
| 4 | Jobs | Live progress, cancellation, and resumable generation. |
| 5 | TTS | Generate per-speaker local voice clips. |
| 6 | MP3 Export | Stitch and download podcast audio. |
| 7 | Polish | Add intro/outro, transitions, normalization, metadata. |
| 8 | Advanced Modes | Add distinct show formats and source-aware generation. |
| 9 | Video | Export MP4 with waveform and captions. |
| 10 | Voice Cloning | Add consent-based custom voice profiles. |

## Immediate Next Actions

1. Scaffold the .NET solution and Angular app.
2. Implement the backend domain models and service interfaces.
3. Build a fake LLM adapter so the full UI flow can be developed without Ollama.
4. Implement project folder persistence for metadata, outline, and transcript artifacts.
5. Build the first end-to-end transcript MVP with real Ollama integration behind a configuration switch.
