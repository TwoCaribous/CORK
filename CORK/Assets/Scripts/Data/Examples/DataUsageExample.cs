using System.Collections.Generic;
using UnityEngine;
using CORK.Data.Rooms;
using CORK.Data.Characters;
using CORK.Data.Dialogue;

namespace CORK.Data.Examples
{
    /// <summary>
    /// Demonstrates how the CORK data architecture is used at runtime.
    /// Attach this MonoBehaviour to any GameObject in a scene and assign
    /// the ScriptableObject fields in the Inspector to see output in the Console.
    ///
    /// This class is for demonstration only — not part of the game's runtime systems.
    /// </summary>
    public class DataUsageExample : MonoBehaviour
    {
        [Header("Room Graph Example")]
        [Tooltip("The room the player is currently in. Assign any RoomData asset.")]
        [SerializeField] private RoomData currentRoom;

        [Header("Random Character Example")]
        [Tooltip("A RandomCharacterData asset with ambient lines populated.")]
        [SerializeField] private RandomCharacterData randomCharacter;

        private void Start()
        {
            DemonstrateRoom();
            DemonstrateRandomCharacter();
        }

        // ── Example 1: Room graph and move command ────────────────────────────────

        /// <summary>
        /// Shows how to retrieve connected room names for 'move' command display,
        /// and how to find a specific room by name for 'move &lt;room name&gt;'.
        /// </summary>
        private void DemonstrateRoom()
        {
            if (currentRoom == null)
            {
                Debug.LogWarning("[CORK] No current room assigned to DataUsageExample.");
                return;
            }

            Debug.Log($"[CORK] You are in: {currentRoom.roomName} (Floor {currentRoom.floorNumber})");
            Debug.Log($"[CORK] {currentRoom.description}");

            // --- 'move' command: display all visible connected rooms ---
            List<string> exits = currentRoom.GetConnectedRoomNames();

            if (exits.Count == 0)
            {
                Debug.Log("[CORK] There are no visible exits from this room.");
            }
            else
            {
                Debug.Log("[CORK] You can move to:");
                foreach (string roomName in exits)
                    Debug.Log($"  - {roomName}");
            }

            // --- 'move <room name>' command: find a specific exit by name ---
            // In the real parser, targetName would come from the player's input.
            string targetName = exits.Count > 0 ? exits[0] : null;

            if (targetName != null)
            {
                RoomConnection connection = currentRoom.FindConnectionByName(targetName);

                if (connection == null)
                {
                    Debug.Log($"[CORK] No exit found named '{targetName}'.");
                }
                else if (connection.isLocked)
                {
                    Debug.Log($"[CORK] The way to {connection.RoomName} is locked.");
                }
                else
                {
                    // In the real system, the parser would set currentRoom = connection.connectedRoom
                    Debug.Log($"[CORK] Moving to: {connection.RoomName}");
                }
            }
        }

        // ── Example 2: Random character ambient dialogue ──────────────────────────

        /// <summary>
        /// Shows how a random character selects and delivers an ambient line.
        /// </summary>
        private void DemonstrateRandomCharacter()
        {
            if (randomCharacter == null)
            {
                Debug.LogWarning("[CORK] No random character assigned to DataUsageExample.");
                return;
            }

            DialogueLine line = randomCharacter.GetRandomAmbientLine();

            if (line == null)
            {
                Debug.Log($"[CORK] {randomCharacter.characterName} has nothing to say.");
            }
            else
            {
                Debug.Log($"[CORK] {line.speakerName}: \"{line.text}\"");
            }
        }
    }
}
