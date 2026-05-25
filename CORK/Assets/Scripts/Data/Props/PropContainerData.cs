using System.Collections.Generic;
using UnityEngine;

namespace CORK.Data.Props
{
    /// <summary>
    /// A prop that also acts as a container for other props.
    /// Inherits from PropData so it fits naturally in any room's props list
    /// and works with existing Inspect, Look, and Take systems without modification.
    ///
    /// Create via: Assets > Create > CORK > Prop Container
    /// </summary>
    [CreateAssetMenu(menuName = "CORK/Prop Container", fileName = "New Prop Container")]
    public class PropContainerData : PropData
    {
        // ── Container State ───────────────────────────────────────────────────────

        [Header("Container")]
        [Tooltip("Whether the container is currently open. Toggled at runtime by the open command.")]
        public bool isOpen;

        [Tooltip("Whether the container is locked and cannot be opened.")]
        public bool isLocked;

        [Tooltip("Message shown when the player tries to open this container while locked. " +
                 "Leave empty for a default message.")]
        public string lockedMessage;

        // ── Contents ─────────────────────────────────────────────────────────────

        [Tooltip("Props inside this container. Listed to the player when the container is opened.")]
        public List<PropData> containedProps = new List<PropData>();

        [Tooltip("The prop the player must use (via the 'use' command) to unlock this container. " +
                 "Leave empty if no key is required. The key is never consumed on use.")]
        public PropData requiredKey;

        // ── Future Expansion Stubs ────────────────────────────────────────────────
        // Uncomment and extend these as new systems are added:
        //
        // [Tooltip("ID matched against a key prop's unlockId to unlock this container.")]
        // public string lockId;
        //
        // [Tooltip("If true, contents are removed from the list after the first time they are taken.")]
        // public bool oneTimeLoot;
        //
        // [Tooltip("If true, contents are not listed until the player searches the container.")]
        // public bool hiddenContents;
        //
        // [Tooltip("If true, opening this container triggers a trap effect.")]
        // public bool isTrapped;
        // ─────────────────────────────────────────────────────────────────────────
    }
}
