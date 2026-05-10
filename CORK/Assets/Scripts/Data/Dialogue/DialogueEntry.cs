using System.Collections.Generic;
using UnityEngine;
using CORK.Data.Dialogue;

namespace CORK.Data.Dialogue
{
    /// <summary>
    /// A reusable dialogue asset that holds an ordered sequence of DialogueLines.
    /// Assigned by reference to EssentialCharacterData (and potentially other systems)
    /// so the same conversation can be shared across characters or triggers.
    /// </summary>
    [CreateAssetMenu(menuName = "CORK/Dialogue Entry", fileName = "New Dialogue Entry")]
    public class DialogueEntry : ScriptableObject
    {
        [Tooltip("The ordered lines that make up this dialogue. Spoken top-to-bottom.")]
        public List<DialogueLine> lines = new List<DialogueLine>();

        // ── Future expansion stubs ───────────────────────────────────────────────
        // When branching dialogue is implemented, each DialogueLine's choices[]
        // can reference additional DialogueEntry assets, forming a dialogue graph.
        //
        // public string dialogueId;            // for save/load state tracking
        // public bool repeatable = true;        // whether this can be replayed
        // public string prerequisiteQuestId;    // quest gate, if needed
        // ────────────────────────────────────────────────────────────────────────
    }
}
