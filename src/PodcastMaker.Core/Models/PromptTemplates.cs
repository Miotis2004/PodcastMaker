namespace PodcastMaker.Core.Models;

public static class PromptTemplates
{
    private static string GetStyleInstructions(string style)
    {
        return style switch
        {
            "Educational" => "Focus on clear explanations, step-by-step breakdowns, and verifying facts. The tone should be informative and structured.",
            "Casual Podcast" => "Focus on conversational flow, personal anecdotes, and a relaxed, friendly atmosphere. Tangents are okay.",
            "Debate" => "Focus on opposing viewpoints, challenging arguments, and rigorous back-and-forth. The tone should be intense but respectful.",
            "Technical Deep Dive" => "Focus on advanced concepts, precise terminology, and deep technical details. Assume an expert audience.",
            "Storytelling" => "Focus on narrative arc, setting a scene, and emotional engagement. The tone should be dramatic and immersive.",
            "Comedy" => "Focus on jokes, witty banter, and humorous observations. The tone should be lighthearted and funny.",
            _ => "Maintain a standard conversational tone."
        };
    }

    public static string OutlinePrompt(string topic, string style, int lengthMinutes, string hostName, string guestName) =>
$@"You are a podcast producer.

Create a structured episode outline for a two-person podcast.

Topic:
{topic}

Style:
{style} - {GetStyleInstructions(style)}

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

    public static string DialogueSegmentPrompt(string episodeTitle, string topic, string style, string segmentTitle, string segmentPurpose, string previousSummary, string hostProfile, string guestProfile) =>
$@"You are writing a natural podcast transcript.

Episode title:
{episodeTitle}

Topic:
{topic}

Style Guidelines:
{style} - {GetStyleInstructions(style)}

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
