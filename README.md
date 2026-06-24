# PodcastMaker# AI Podcast Generator: Development Document

## 1. Project Overview

The AI Podcast Generator is a local-first application that allows a user to enter a topic and generate a long-form podcast-style conversation between two AI speakers. The system will use a local LLM through Ollama for script generation and a local or optional external text-to-speech engine for audio generation.

The first version should focus on generating strong long-form dialogue. Audio generation can be added after the transcript engine is stable.

## 2. Working Title Options

Recommended working title:

**DialogCast**

Other options:

* EchoCast
* Synthetic Roundtable
* Two Voices
* CastForge
* LLM Podcast Studio
* DeepTalk
* PodSmith

## 3. Core Concept

The user provides a topic, chooses a conversation style, selects a length, and starts generation.

Example topic:

> The rise of symphonic death metal

The app generates a structured dialogue between two speakers:

* Host
* Expert Guest

The output can be:

* Podcast transcript
* Audio podcast
* Downloadable MP3
* Optional video with waveform and captions

## 4. Primary Goals

The app should:

1. Generate long-form, coherent two-person conversations.
2. Support podcast-style structure.
3. Allow topic, tone, length, and speaker personalities to be configured.
4. Produce clean transcripts.
5. Convert the transcript into realistic speech.
6. Export audio and text files.
7. Run locally whenever possible.

## 5. Recommended Technology Stack

### Frontend

* Angular
* TypeScript
* HTML/CSS
* Optional: Tailwind CSS or Angular Material

### Backend

* ASP.NET Core
* C#
* REST API
* SignalR for live generation updates

### LLM Runtime

* Ollama

Ollama exposes a local API once running, commonly available at `localhost:11434`, and supports generation through local models. This makes it a good fit for a private, local-first app.

### TTS Options

Recommended order:

1. Piper TTS for the first working version
2. Kokoro TTS for better naturalness
3. XTTS-style voice cloning later, only after the core app is stable

Piper is a fast local neural text-to-speech system, while Kokoro is a lightweight open-weight TTS model designed for strong voice quality at relatively low model size.

### Audio Processing

* FFmpeg

FFmpeg should be used for stitching generated voice clips, converting WAV to MP3, normalizing audio, and creating optional video output.

### Database

Start with:

* SQLite

Later:

* PostgreSQL

### Local File Storage

Store generated projects as folders:

```text
/projects
  /episode-id
    metadata.json
    outline.json
    transcript.json
    transcript.txt
    audio
      host_001.wav
      guest_001.wav
      final.mp3
```

## 6. MVP Feature Set

The MVP should include:

1. Topic input
2. Speaker selection
3. Podcast length selection
4. Tone/style selection
5. Transcript generation
6. Transcript viewer
7. Save transcript
8. Regenerate episode
9. Export transcript as TXT or Markdown

Do not include TTS in the first milestone unless the transcript generation is already reliable.

## 7. Core User Flow

1. User opens app.
2. User enters a topic.
3. User chooses length:

   * 5 minutes
   * 15 minutes
   * 30 minutes
   * Custom
4. User chooses style:

   * Educational
   * Casual podcast
   * Debate
   * Technical deep dive
   * Storytelling
   * Comedy
5. User chooses speakers:

   * Host
   * Expert
6. App generates an episode outline.
7. App generates dialogue section by section.
8. User reviews transcript.
9. User optionally generates audio.
10. User exports transcript or MP3.

## 8. Conversation Generation Strategy

Avoid generating an entire long podcast in one prompt. Instead, generate in phases.

### Step 1: Topic Analysis

The system analyzes the topic and creates a clear episode direction.

Output:

```json
{
  "topic": "The history of black metal",
  "angle": "A historical and cultural overview",
  "targetAudience": "Music fans",
  "tone": "Serious but conversational"
}
```

### Step 2: Episode Outline

Generate sections:

```json
{
  "segments": [
    {
      "title": "Opening",
      "purpose": "Introduce the topic"
    },
    {
      "title": "Origins",
      "purpose": "Discuss early influences"
    },
    {
      "title": "Second Wave",
      "purpose": "Explore the genre's defining era"
    },
    {
      "title": "Modern Black Metal",
      "purpose": "Discuss current evolution"
    },
    {
      "title": "Closing Thoughts",
      "purpose": "Summarize the discussion"
    }
  ]
}
```

### Step 3: Segment Dialogue

Generate each segment separately.

Each segment should include:

* Host questions
* Guest answers
* Natural transitions
* Occasional disagreement
* Short recap moments
* No repeated introductions

### Step 4: Continuity Pass

After all sections are generated, run a cleanup pass to:

* Remove repetition
* Smooth transitions
* Fix speaker consistency
* Check factual coherence
* Improve pacing

## 9. Speaker Model

Each speaker should have a profile.

### Host

```json
{
  "name": "Alex",
  "role": "Host",
  "personality": "curious, clear, conversational",
  "speakingStyle": "asks thoughtful questions and keeps the discussion moving"
}
```

### Guest

```json
{
  "name": "Morgan",
  "role": "Expert Guest",
  "personality": "knowledgeable, opinionated, detailed",
  "speakingStyle": "answers with stories, examples, and occasional disagreement"
}
```

## 10. Episode Length Targets

Approximate word counts:

* 5 minutes: 700-900 words
* 15 minutes: 2,100-2,700 words
* 30 minutes: 4,200-5,400 words
* 60 minutes: 8,500-10,500 words

Generation should be segment-based so the app does not rely on one massive prompt.

## 11. Phase 1: Transcript MVP

Goal:

Build the app that generates readable podcast transcripts.

Features:

* Angular topic input page
* ASP.NET Core API
* Ollama connection
* Episode outline generation
* Segment-by-segment transcript generation
* Transcript display
* Save transcript to local file
* Basic project history

Deliverables:

* Working Angular UI
* Backend API
* Ollama integration
* Transcript export

Success criteria:

* User can generate a 5-15 minute podcast transcript.
* Dialogue is formatted cleanly.
* Speakers stay consistent.
* User can save or copy the transcript.

## 12. Phase 2: Better Podcast Structure

Goal:

Improve quality and coherence.

Features:

* Episode outline editor
* Speaker profile editor
* Tone selector
* Episode style selector
* Regenerate individual segment
* Rewrite selected passage
* Add intro and outro templates
* Add segment transitions
* Add episode title generation
* Add show notes generation

Deliverables:

* Editable episode outline
* Improved transcript pipeline
* Better prompt templates
* Segment regeneration

Success criteria:

* User can shape the podcast before generation.
* Long episodes feel more structured.
* Regeneration does not require starting over.

## 13. Phase 3: TTS Audio Generation

Goal:

Convert transcript into spoken audio.

Features:

* Assign one TTS voice to Host
* Assign one TTS voice to Guest
* Generate WAV per dialogue line or paragraph
* Stitch audio into final episode
* Export WAV and MP3
* Add silence between turns
* Normalize volume
* Save generated audio files

Recommended first TTS engine:

* Piper

Recommended upgrade option:

* Kokoro

Deliverables:

* TTS service
* Audio generation queue
* FFmpeg stitching
* MP3 export

Success criteria:

* User can generate an MP3 from a transcript.
* Host and Guest use different voices.
* Audio is understandable and reasonably paced.

## 14. Phase 4: Audio Polish

Goal:

Make the generated podcast sound more realistic.

Features:

* Add intro music
* Add outro music
* Add transition stingers
* Add room tone
* Add configurable pauses
* Add laughter or reaction markers carefully
* Add pronunciation dictionary
* Add voice speed and pitch controls
* Add automatic audio loudness normalization

Optional:

* Generate chapter markers
* Export podcast-ready metadata
* Add ID3 tags

Success criteria:

* Output feels closer to a finished podcast.
* Audio levels are consistent.
* Voices sound less robotic.

## 15. Phase 5: Project Library

Goal:

Make the app useful over time.

Features:

* Episode dashboard
* Search previous episodes
* Sort by topic, date, length, status
* Save speaker presets
* Save show templates
* Favorite voices
* Duplicate episode
* Resume unfinished generation
* Delete episode

Database tables:

* Episodes
* Segments
* Speakers
* Voices
* AudioFiles
* GenerationJobs
* Settings

Success criteria:

* User can manage many podcast projects.
* Past episodes are easy to reopen and export.

## 16. Phase 6: Advanced Generation

Goal:

Improve intelligence and customization.

Features:

* Debate mode
* Interview mode
* Explainer mode
* Comedy mode
* Documentary mode
* Storytelling mode
* Multi-source mode
* Fact-check pass
* User-provided notes
* User-provided documents
* Custom system prompts
* Model selection per speaker

Example:

```text
Host uses llama3.1
Guest uses qwen
Critic pass uses mistral
```

Success criteria:

* Different show formats feel meaningfully different.
* User can control the style without editing prompts manually.

## 17. Phase 7: Optional Video Export

Goal:

Create YouTube-ready output.

Features:

* Static background image
* Speaker avatars
* Waveform animation
* Captions
* Episode title card
* Progress bar
* Export MP4

Video generation stack:

* FFmpeg
* Optional: Remotion
* Optional: Angular-rendered canvas capture

Success criteria:

* User can export an MP4 suitable for YouTube.
* Captions align reasonably with the audio.

## 18. Phase 8: Optional Voice Cloning

Goal:

Allow custom speaker voices.

Features:

* Voice sample upload
* Voice training/import workflow
* Voice profile library
* Consent warning
* Local-only voice storage
* Per-speaker cloned voice assignment

Important restrictions:

* Only allow the user to clone voices they own or have permission to use.
* Include clear consent messaging.
* Store voice models locally by default.
* Do not provide tools for impersonation or fraud.

Success criteria:

* User can create a custom voice profile.
* Generated audio uses that voice consistently.
* App avoids unsafe or misleading voice use.

## 19. API Design

### Generate Outline

```http
POST /api/episodes/outline
```

Body:

```json
{
  "topic": "The future of electric guitars",
  "style": "casual podcast",
  "lengthMinutes": 15,
  "host": "Alex",
  "guest": "Morgan"
}
```

### Generate Segment

```http
POST /api/episodes/{episodeId}/segments/{segmentId}/generate
```

### Generate Full Transcript

```http
POST /api/episodes/{episodeId}/generate-transcript
```

### Generate Audio

```http
POST /api/episodes/{episodeId}/generate-audio
```

### Export

```http
GET /api/episodes/{episodeId}/export/transcript
GET /api/episodes/{episodeId}/export/audio
```

## 20. Backend Services

### EpisodeService

Handles episode creation, metadata, and persistence.

### OutlineService

Generates structured episode outlines.

### DialogueGenerationService

Generates dialogue segment by segment.

### OllamaService

Handles communication with Ollama.

### TranscriptService

Formats, edits, and exports transcript output.

### TtsService

Converts dialogue lines to speech.

### AudioAssemblyService

Uses FFmpeg to stitch, normalize, and export audio.

### ProjectStorageService

Manages local episode folders.

### JobQueueService

Runs long generation tasks without blocking the UI.

## 21. Frontend Pages

### Home Page

* Topic input
* New episode button
* Recent projects

### Episode Setup Page

* Topic
* Length
* Style
* Speaker selection
* Voice selection
* Advanced settings

### Outline Page

* View generated outline
* Edit segments
* Add/remove/reorder segments

### Transcript Page

* View transcript
* Regenerate segment
* Rewrite selection
* Export transcript

### Audio Page

* Generate audio
* Preview audio
* Regenerate individual voice line
* Export MP3

### Library Page

* View saved episodes
* Search
* Open
* Duplicate
* Delete

### Settings Page

* Ollama URL
* Default model
* TTS engine
* FFmpeg path
* Output folder
* Default voices

## 22. Prompt Template: Outline

```text
You are a podcast producer.

Create a structured episode outline for a two-person podcast.

Topic:
{topic}

Style:
{style}

Target length:
{lengthMinutes} minutes

Speakers:
Host: {hostName}
Guest: {guestName}

Return JSON only.

The outline should include:
- episode title
- short description
- 5 to 8 segments
- purpose of each segment
- estimated duration for each segment
- key talking points
```

## 23. Prompt Template: Dialogue Segment

```text
You are writing a natural podcast transcript.

Episode title:
{episodeTitle}

Topic:
{topic}

Current segment:
{segmentTitle}

Segment purpose:
{segmentPurpose}

Previous summary:
{previousSummary}

Speakers:
Host: {hostProfile}
Guest: {guestProfile}

Write only this segment.

Rules:
- Use natural two-person podcast dialogue.
- Keep speaker names consistent.
- Avoid repeating the full episode introduction.
- Include follow-up questions.
- Allow occasional disagreement or clarification.
- Do not include stage directions unless requested.
- Format each line as Speaker: dialogue.
```

## 24. Prompt Template: Cleanup Pass

```text
Edit the following podcast transcript.

Goals:
- Remove repetition.
- Improve flow.
- Preserve meaning.
- Keep speaker voices distinct.
- Keep the transcript natural.
- Do not shorten aggressively.

Transcript:
{transcript}
```

## 25. Data Model

### Episode

```csharp
public class Episode
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Topic { get; set; }
    public string Style { get; set; }
    public int LengthMinutes { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; }
}
```

### Speaker

```csharp
public class Speaker
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    public string Personality { get; set; }
    public string SpeakingStyle { get; set; }
    public string VoiceId { get; set; }
}
```

### Segment

```csharp
public class Segment
{
    public Guid Id { get; set; }
    public Guid EpisodeId { get; set; }
    public string Title { get; set; }
    public int SortOrder { get; set; }
    public string Purpose { get; set; }
    public string Transcript { get; set; }
}
```

### DialogueLine

```csharp
public class DialogueLine
{
    public Guid Id { get; set; }
    public Guid SegmentId { get; set; }
    public string SpeakerName { get; set; }
    public string Text { get; set; }
    public string AudioPath { get; set; }
    public int SortOrder { get; set; }
}
```

## 26. Generation Job States

Use clear statuses:

```text
Draft
OutlineGenerated
GeneratingTranscript
TranscriptGenerated
GeneratingAudio
AudioGenerated
Exported
Failed
```

## 27. Error Handling

The app should handle:

* Ollama not running
* Model not installed
* LLM timeout
* Invalid JSON from model
* TTS engine missing
* FFmpeg missing
* Audio generation failure
* Disk write failure
* User cancellation

For long jobs, keep partial output.

## 28. Local-First Privacy Model

The app should default to local processing.

Local-first design:

* Ollama runs locally.
* TTS runs locally.
* Episodes are stored locally.
* No cloud account required.
* No telemetry by default.
* User controls output folder.

Optional cloud features can be added later, but they should not be required.

## 29. Safety and Content Controls

The app should include:

* Content warning settings
* User-controlled filtering level
* Voice cloning consent requirements
* No impersonation mode
* Clear labeling that audio is AI-generated
* Optional generated audio watermark metadata

## 30. Suggested Development Order

### Milestone 1

Create Angular app and ASP.NET Core API.

### Milestone 2

Connect backend to Ollama.

### Milestone 3

Generate episode outlines.

### Milestone 4

Generate transcript segments.

### Milestone 5

Save and reload projects.

### Milestone 6

Add transcript editor.

### Milestone 7

Add Piper TTS.

### Milestone 8

Add FFmpeg audio stitching.

### Milestone 9

Add MP3 export.

### Milestone 10

Add project library.

### Milestone 11

Add Kokoro TTS option.

### Milestone 12

Add optional video export.

## 31. Minimum First Prototype

The first prototype should do only this:

1. User enters topic.
2. User chooses 5-minute or 15-minute length.
3. Backend generates outline.
4. Backend generates transcript.
5. UI displays transcript.
6. User can copy or save transcript.

This is enough to prove the core idea.

## 32. Best First Demo Topic

Use a topic that naturally fits your interests:

> The evolution of symphonic death metal and why bands mix orchestral beauty with extreme vocals.

This will quickly reveal whether the app can generate engaging long-form discussion.

## 33. Future Expansion Ideas

* Three-person panel mode
* Fictional character podcast mode
* RPG tavern conversation mode
* Music analysis mode
* Dissertation research explainer mode
* YouTube script generator
* Chaptered audiobook-style conversations
* AI debate coach
* Custom recurring show templates

## 34. Final Product Vision

The final version should feel like a local AI podcast studio.

The user gives the app a topic, chooses the hosts, chooses the style, and receives a polished transcript or finished audio episode. The strongest version is not just a chatbot. It is a content generation studio for long-form AI dialogue.

## 14. Development Commands

### Backend

From the repository root:

```bash
dotnet restore PodcastMaker.sln
dotnet build PodcastMaker.sln
dotnet test PodcastMaker.sln
dotnet run --project src/PodcastMaker.Api
```

The API exposes:

* `GET /api/health` for application health.
* `GET /api/health/ollama` for Ollama connectivity.
* `GET /api/config` for safe development configuration values.

### Frontend

From `src/PodcastMaker.Web`:

```bash
npm install
npm start
npm run build
```

The Angular shell reads its API base URL from `src/environments/environment.ts` and displays the API health response on the landing page.

### Formatting

The repository uses `.editorconfig` for shared C# and TypeScript formatting defaults. Use your IDE's EditorConfig support or the relevant framework formatter before committing changes.
