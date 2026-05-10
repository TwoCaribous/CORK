using System;
using UnityEngine;

namespace CORK.Data.Dialogue
{
    /// <summary>
    /// A single line of dialogue spoken by a character.
    /// Kept as a plain serializable class (not a ScriptableObject) so it can live
    /// inline inside DialogueEntry and RandomCharacterData lists without cluttering
    /// the Project window with individual assets.
    /// </summary>
    [Serializable]
    public class DialogueLine
    {
        [Tooltip("The name displayed as the speaker of this line. Usually matches the character's name.")]
        public string speakerName;

        [TextArea(2, 6)]
        [Tooltip("The text content of this dialogue line.")]
        public string text;

        // ── Future expansion stubs ───────────────────────────────────────────────
        // Uncomment and expand these when branching dialogue, conditions, and
        // choices are implemented.
        //
        // [Tooltip("Optional condition key that must be met before this line is shown.")]
        // public string conditionKey;
        //
        // [Tooltip("Choices available to the player after this line is delivered.")]
        // public DialogueChoice[] choices;
        // ────────────────────────────────────────────────────────────────────────
    }
}
