using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CORK.Data.Props;
using CORK.Data.Characters;

namespace CORK.Data.Rooms
{
    /// <summary>
    /// A node in the game's room graph. Each RoomData ScriptableObject represents
    /// one location the player can visit. Rooms connect to other rooms via a list
    /// of RoomConnections, forming a graph that supports any number of exits.
    ///
    /// Create via: Assets > Create > CORK > Room
    /// </summary>
    [CreateAssetMenu(menuName = "CORK/Room", fileName = "New Room")]
    public class RoomData : ScriptableObject
    {
        // ── Identity ─────────────────────────────────────────────────────────────

        [Tooltip("The display name of this room, shown to the player and used for navigation lookup.")]
        public string roomName;

        [TextArea(3, 8)]
        [Tooltip("The descriptive text shown when the player enters or looks around this room.")]
        public string description;

        [Tooltip("Which floor of the dungeon/building this room is on.")]
        public int floorNumber;

        [Tooltip("Optional image displayed when the player is in this room.")]
        public Sprite roomImage;

        // ── Contents ─────────────────────────────────────────────────────────────

        [Tooltip("Props (objects) present in this room.")]
        public List<PropData> props = new List<PropData>();

        [Tooltip("Characters and NPCs present in this room. Accepts both Essential and Random character types.")]
        public List<CharacterData> characters = new List<CharacterData>();

        // ── Connections ───────────────────────────────────────────────────────────

        [Tooltip("All exits from this room. Each entry links to another RoomData asset " +
                 "and optionally carries direction, lock, and visibility metadata.")]
        public List<RoomConnection> connections = new List<RoomConnection>();

        // ── Graph Helpers ─────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the display names of all visible, non-null connected rooms.
        /// Use this to populate the list shown to the player when they type 'move'.
        /// Hidden connections are excluded until revealed by gameplay logic.
        /// </summary>
        public List<string> GetConnectedRoomNames()
        {
            return connections
                .Where(c => !c.isHidden && c.connectedRoom != null)
                .Select(c => c.RoomName)
                .ToList();
        }

        /// <summary>
        /// Finds a connection by the name of its destination room.
        /// Case-insensitive. Returns null if no visible match is found.
        /// Use this when the player types 'move &lt;room name&gt;' directly.
        /// </summary>
        public RoomConnection FindConnectionByName(string targetName)
        {
            return connections.FirstOrDefault(c =>
                !c.isHidden &&
                c.connectedRoom != null &&
                string.Equals(c.RoomName, targetName, System.StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Finds a connection by direction label (e.g. "north", "up").
        /// Case-insensitive. Returns null if no match is found.
        /// Intended for future directional parser support.
        /// </summary>
        public RoomConnection FindConnectionByDirection(string dir)
        {
            return connections.FirstOrDefault(c =>
                c.connectedRoom != null &&
                string.Equals(c.direction, dir, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}
