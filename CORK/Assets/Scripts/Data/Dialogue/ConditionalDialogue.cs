using System;
using UnityEngine;
using CORK.Data.Inventory;

namespace CORK.Data.Dialogue
{
    /// <summary>
    /// Pairs a DialogueCondition with a DialogueEntry.
    /// Used in EssentialCharacterData to create a priority-ordered list of
    /// conditional dialogue options — the first entry whose condition passes is played.
    ///
    /// Kept as a plain serializable class so it lives inline in the character asset
    /// without creating extra Project-window clutter.
    /// </summary>
    [Serializable]
    public class ConditionalDialogue
    {
        [Tooltip("The condition that must be met for this dialogue to be selected. " +
                 "Set conditionType to None to make this entry unconditional.")]
        public DialogueCondition condition = new DialogueCondition();

        [Tooltip("The dialogue played when the condition passes.")]
        public DialogueEntry dialogue;

        /// <summary>
        /// Convenience wrapper — evaluates the condition and returns whether
        /// this entry's dialogue should be used.
        /// </summary>
        public bool ConditionPasses(PlayerInventoryData inventory, CORK.Data.GameFlags flags = null)
        {
            return condition != null && condition.Evaluate(inventory, flags);
        }
    }
}
