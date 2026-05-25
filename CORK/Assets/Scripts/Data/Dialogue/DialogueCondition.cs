using System;
using UnityEngine;
using CORK.Data.Props;
using CORK.Data.Inventory;

namespace CORK.Data.Dialogue
{
    /// <summary>
    /// The types of conditions that can gate a dialogue entry.
    /// Extend this enum as new game systems (quests, flags, etc.) are added.
    /// </summary>
    public enum DialogueConditionType
    {
        /// <summary>No condition — this dialogue always passes. Use for an unconditional fallback.</summary>
        None,

        /// <summary>Passes when the player has a specific prop in their inventory.</summary>
        HasItem,

        /// <summary>Passes when the player does NOT have a specific prop in their inventory.</summary>
        DoesNotHaveItem,

        /// <summary>Passes when a named world flag is active (set via GameFlags.SetFlag).</summary>
        HasFlag,

        /// <summary>Passes when a named world flag is NOT active.</summary>
        DoesNotHaveFlag,
    }

    /// <summary>
    /// A serializable condition that can be evaluated against the player's current state.
    /// Pair this with a DialogueEntry inside a ConditionalDialogue to gate dialogue behind
    /// inventory checks (and, in the future, quest flags, visited rooms, etc.).
    /// </summary>
    [Serializable]
    public class DialogueCondition
    {
        [Tooltip("The type of condition to evaluate.")]
        public DialogueConditionType conditionType = DialogueConditionType.None;

        [Tooltip("The prop required in (or absent from) the player's inventory. " +
                 "Only used when conditionType is HasItem or DoesNotHaveItem.")]
        public PropData requiredProp;

        [Tooltip("The flag name to check against GameFlags. " +
                 "Only used when conditionType is HasFlag or DoesNotHaveFlag.")]
        public string requiredFlag;

        /// <summary>
        /// Evaluates this condition against the player's current inventory and world flags.
        /// Null inventory is treated as empty; null flags are treated as no flags set.
        /// </summary>
        public bool Evaluate(PlayerInventoryData inventory, CORK.Data.GameFlags flags = null)
        {
            switch (conditionType)
            {
                case DialogueConditionType.None:
                    return true;

                case DialogueConditionType.HasItem:
                    return inventory != null && inventory.HasItem(requiredProp);

                case DialogueConditionType.DoesNotHaveItem:
                    return inventory == null || !inventory.HasItem(requiredProp);

                case DialogueConditionType.HasFlag:
                    return flags != null && flags.HasFlag(requiredFlag);

                case DialogueConditionType.DoesNotHaveFlag:
                    return flags == null || !flags.HasFlag(requiredFlag);

                default:
                    return false;
            }
        }
    }
}
