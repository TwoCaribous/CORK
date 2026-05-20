using System.Collections.Generic;
using UnityEngine;
using CORK.Data.Props;

namespace CORK.Data.Inventory
{
    /// <summary>
    /// A runtime ScriptableObject that represents the player's current inventory.
    /// Create a single instance via Assets > Create > CORK > Player Inventory and
    /// reference it wherever inventory checks are needed (characters, triggers, UI).
    ///
    /// Because this is a ScriptableObject, changes persist across scene loads but
    /// reset when Play Mode ends — call Clear() at the start of a new game session.
    ///
    /// Create via: Assets > Create > CORK > Player Inventory
    /// </summary>
    [CreateAssetMenu(menuName = "CORK/Player Inventory", fileName = "PlayerInventory")]
    public class PlayerInventoryData : ScriptableObject
    {
        [Tooltip("Props currently held by the player. Do not edit directly at runtime — use AddItem / RemoveItem.")]
        public List<PropData> items = new List<PropData>();

        // ── Queries ───────────────────────────────────────────────────────────────

        /// <summary>Returns true if the player currently holds the given prop.</summary>
        public bool HasItem(PropData prop) => prop != null && items.Contains(prop);

        /// <summary>Returns true if the player holds a prop with the given name (case-insensitive).</summary>
        public bool HasItemByName(string propName)
        {
            if (string.IsNullOrEmpty(propName)) return false;
            return items.Exists(p => string.Equals(p.propName, propName, System.StringComparison.OrdinalIgnoreCase));
        }

        // ── Mutations ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Adds a prop to the inventory.
        /// Returns true on success, false if the prop is null or already present.
        /// </summary>
        public bool AddItem(PropData prop)
        {
            if (prop == null || items.Contains(prop)) return false;

            items.Add(prop);
            return true;
        }

        /// <summary>
        /// Removes a prop from the inventory.
        /// Returns true on success, false if the prop was not present.
        /// </summary>
        public bool RemoveItem(PropData prop)
        {
            return prop != null && items.Remove(prop);
        }

        /// <summary>Removes all items from the inventory. Call this when starting a new game.</summary>
        public void Clear() => items.Clear();
    }
}
