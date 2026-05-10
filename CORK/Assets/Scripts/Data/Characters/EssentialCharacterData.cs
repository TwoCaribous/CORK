using UnityEngine;
using CORK.Data.Dialogue;

namespace CORK.Data.Characters
{
    /// <summary>
    /// A story-critical or quest-relevant character with meaningful dialogue.
    /// Assign a DialogueEntry to define their primary conversation.
    /// </summary>
    [CreateAssetMenu(menuName = "CORK/Character/Essential", fileName = "New Essential Character")]
    public class EssentialCharacterData : CharacterData
    {
        [Tooltip("The primary dialogue that plays when the player interacts with this character.")]
        public DialogueEntry primaryDialogue;

        // ── Future expansion stubs ───────────────────────────────────────────────
        // When quests and progression systems are implemented, add fields here:
        //
        // public QuestData associatedQuest;
        // public string prerequisiteFlag;      // condition that must be true to interact
        // public DialogueEntry completionDialogue;
        // ────────────────────────────────────────────────────────────────────────
    }
}
