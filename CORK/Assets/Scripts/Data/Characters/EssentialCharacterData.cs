using System.Collections.Generic;
using UnityEngine;
using CORK.Data.Dialogue;
using CORK.Data.Inventory;

namespace CORK.Data.Characters
{
    /// <summary>
    /// A story-critical or quest-relevant character with meaningful, condition-gated dialogue.
    ///
    /// How dialogue resolution works:
    ///   1. conditionalDialogues is evaluated top-to-bottom.
    ///   2. The first entry whose condition passes is played.
    ///   3. If no conditional entry passes, primaryDialogue is used as a fallback.
    ///
    /// Typical setup:
    ///   - Add item-gated entries at the top of conditionalDialogues (e.g. "has key").
    ///   - Leave primaryDialogue as the generic "I don't know you yet" conversation.
    ///
    /// Create via: Assets > Create > CORK > Character > Essential
    /// </summary>
    [CreateAssetMenu(menuName = "CORK/Character/Essential", fileName = "New Essential Character")]
    public class EssentialCharacterData : CharacterData
    {
        [Header("Conditional Dialogue")]
        [Tooltip("Dialogue entries paired with conditions. Evaluated top-to-bottom; " +
                 "the first entry whose condition passes is played. " +
                 "Use conditionType = None for an unconditional entry in this list.")]
        public List<ConditionalDialogue> conditionalDialogues = new List<ConditionalDialogue>();

        [Header("Item Interactions")]
        [Tooltip("Responses triggered when the player gives this character a specific item. " +
                 "The item is consumed on use. Evaluated in order; first match wins.")]
        public List<ItemInteraction> itemInteractions = new List<ItemInteraction>();

        [Header("Fallback Dialogue")]
        [Tooltip("Played when no conditional dialogue passes. " +
                 "Acts as the default conversation before any story conditions are met.")]
        public DialogueEntry primaryDialogue;

        // ── Future expansion stubs ───────────────────────────────────────────────
        // Additional condition types to add to DialogueConditionType as systems grow:
        //
        // public QuestData associatedQuest;
        // public string prerequisiteFlag;    // general boolean flag system
        // public DialogueEntry completionDialogue;
        // ────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the appropriate DialogueEntry for the current player state.
        /// Evaluates conditionalDialogues in order and returns the first match,
        /// falling back to primaryDialogue if nothing passes.
        /// Pass the player's inventory and game flags for full condition evaluation.
        /// </summary>
        public DialogueEntry GetDialogue(PlayerInventoryData inventory, CORK.Data.GameFlags flags = null)
        {
            foreach (ConditionalDialogue conditional in conditionalDialogues)
            {
                if (conditional.dialogue != null && conditional.ConditionPasses(inventory, flags))
                    return conditional.dialogue;
            }

            return primaryDialogue;
        }
    }
}
