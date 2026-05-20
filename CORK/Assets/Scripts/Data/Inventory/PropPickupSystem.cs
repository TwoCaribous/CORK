using UnityEngine;
using CORK.Data.Props;
using CORK.Data.Rooms;

namespace CORK.Data.Inventory
{
    /// <summary>
    /// Handles the 'take' command: removes a prop from a room's prop list
    /// and places it in the player's inventory.
    ///
    /// Attach to a game-manager or player GameObject. Assign the PlayerInventoryData
    /// asset in the Inspector. Wire your parser/input system to call TryPickUp or
    /// TryPickUpByName when the player types a take command.
    ///
    /// Note: RoomData is a ScriptableObject, so removing from its props list affects
    /// the asset at runtime. Changes persist across scene loads but reset when Play Mode ends.
    /// </summary>
    public class PropPickupSystem : MonoBehaviour
    {
        [Tooltip("The player's inventory asset. Create one via CORK/Player Inventory.")]
        [SerializeField] private PlayerInventoryData playerInventory;

        // ── Public API ────────────────────────────────────────────────────────────

        /// <summary>
        /// Attempts to pick up a specific prop from the given room.
        /// The prop must be present in the room and have canBeTaken set to true.
        /// Returns true if the prop was successfully moved to inventory.
        /// </summary>
        public bool TryPickUp(PropData prop, RoomData room)
        {
            if (!ValidateReferences()) return false;
            if (prop == null || room == null) return false;

            if (!prop.canBeTaken)
            {
                Debug.Log($"[CORK] {prop.propName} cannot be taken.");
                return false;
            }

            if (!room.props.Contains(prop))
            {
                Debug.Log($"[CORK] {prop.propName} is not in {room.roomName}.");
                return false;
            }

            room.props.Remove(prop);
            playerInventory.AddItem(prop);
            Debug.Log($"[CORK] Picked up: {prop.propName}");
            return true;
        }

        /// <summary>
        /// Searches the given room for a prop matching propName (case-insensitive)
        /// and attempts to pick it up. Returns the PropData on success, null on failure.
        /// Suitable for wiring directly to a text parser's 'take &lt;name&gt;' command.
        /// </summary>
        public PropData TryPickUpByName(string propName, RoomData room)
        {
            if (!ValidateReferences()) return null;
            if (room == null || string.IsNullOrEmpty(propName)) return null;

            PropData prop = room.props.Find(p =>
                string.Equals(p.propName, propName, System.StringComparison.OrdinalIgnoreCase));

            if (prop == null)
            {
                Debug.Log($"[CORK] No prop named '{propName}' found in {room.roomName}.");
                return null;
            }

            return TryPickUp(prop, room) ? prop : null;
        }

        // ── Private Helpers ───────────────────────────────────────────────────────

        private bool ValidateReferences()
        {
            if (playerInventory != null) return true;

            Debug.LogError("[CORK] PropPickupSystem: playerInventory is not assigned.", this);
            return false;
        }
    }
}
