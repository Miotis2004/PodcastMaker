namespace PodcastMaker.Core.Models;

public static class PromptTemplates
{
    public static string OutlinePrompt(string topic, string style, int lengthMinutes, string hostName, string guestName) =>
$@"You are a podcast producer.

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
- episode title (string)
- short description (string)
- 5 to 8 segments (array of objects)
  - purpose of each segment (string)
  - estimated duration for each segment (integer in seconds)
  - key talking points (array of strings)
  - title (string)";

    public static string DialogueSegmentPrompt(string episodeTitle, string topic, string segmentTitle, string segmentPurpose, string previousSummary, string hostProfile, string guestProfile) =>
$@"You are writing a natural podcast transcript.

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
- Format each line as Speaker: dialogue.";

    public static string CleanupPrompt(string transcript) =>
$@"Edit the following podcast transcript.

Goals:
- Remove repetition.
- Improve flow.
- Preserve meaning.
- Keep speaker voices distinct.
- Keep the transcript natural.
- Do not shorten aggressively.

Transcript:
{transcript}";
}
