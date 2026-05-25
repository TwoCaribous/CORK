using System;
using UnityEngine;
using CORK.Data.Props;

namespace CORK.Data.Rooms
{
    /// <summary>
    /// Represents a single connection (exit) from one room to another.
    /// Kept as a plain serializable class so it lives inline inside a Room's
    /// connections list — no separate asset required per connection.
    ///
    /// The graph structure is built by assigning RoomData ScriptableObject references
    /// here; rooms form an undirected (or directed) graph depending on how connections
    /// are set up in the Inspector.
    /// </summary>
    [Serializable]
    public class RoomConnection
    {
        [Tooltip("The room this connection leads to.")]
        public RoomData connectedRoom;

        [Tooltip("Optional directional label (e.g. 'north', 'east', 'up'). " +
                 "Leave empty if direction is not relevant for this exit.")]
        public string direction;

        [Tooltip("If true, this exit is locked and cannot be used until unlocked by gameplay logic.")]
        public bool isLocked;

        [Tooltip("Message shown when the player tries to use this exit while locked. Leave empty for a default message.")]
        public string lockedMessage;

        [Tooltip("If true, this exit is hidden and will not appear in the move list " +
                 "until revealed by gameplay logic.")]
        public bool isHidden;

        [Tooltip("If true, the player has already passed through this connection at least once.")]
        public bool hasBeenVisited;

        [Tooltip("A short description of the door or passage (e.g. 'a heavy oak door', 'a narrow gap in the wall').")]
        public string doorDescription;

        [Tooltip("The name shown for this destination in exit listings before the player has visited it. " +
                 "Once visited, the actual room name is shown instead. " +
                 "Falls back to doorDescription if left empty.")]
        public string displayName;

        [Tooltip("The prop the player must use (via the 'use' command) to unlock this connection. " +
                 "Leave empty if no key is required. The key is never consumed on use.")]
        public PropData requiredKey;

        /// <summary>
        /// Convenience accessor for the connected room's name.
        /// Safe to call even if connectedRoom is null.
        /// </summary>
        public string RoomName => connectedRoom != null ? connectedRoom.roomName : string.Empty;
    }
}
