using System.Collections.Generic;
using UnityEngine;
using CORK.Data.Characters;
using CORK.Data.Props;
using CORK.Data.Rooms;

/// <summary>
/// Handles the 'use' command. Applies a carried item to a target in the current room.
/// A target is always required — "use X on Y" is the only accepted form.
///
/// Routing priority:
///   1. Exit / door (RoomConnection) — uses requiredKey to unlock; key stays in inventory.
///   2. Locked container (PropContainerData) — uses requiredKey to unlock; key stays in inventory.
///   3. Character — redirects the player to 'give' instead.
///   4. Any other prop — "Nothing happens."
///   5. Nothing matched — "You don't see that here."
///
/// Create via: Assets > Create > TextAdventure > InputActions > Use
/// Assign to GameController.inputActions[] and set keyWord to "use".
/// </summary>
[CreateAssetMenu(menuName = "TextAdventure/InputActions/Use")]
public class Use : InputAction
{
    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        // Need at least: use <item> on <target>  →  4 words minimum
        if (separatedInputWords.Length < 4)
        {
            controller.LogStringWithReturn("Use what on what? (e.g. 'use key on door')");
            return;
        }

        // Locate the "on" separator
        int onIndex = -1;
        for (int i = 1; i < separatedInputWords.Length; i++)
        {
            if (string.Equals(separatedInputWords[i], "on", System.StringComparison.OrdinalIgnoreCase))
            {
                onIndex = i;
                break;
            }
        }

        if (onIndex < 0 || onIndex == separatedInputWords.Length - 1)
        {
            controller.LogStringWithReturn("Use it on what? (e.g. 'use key on door')");
            return;
        }

        string itemName   = string.Join(" ", separatedInputWords, 1, onIndex - 1);
        string targetName = string.Join(" ", separatedInputWords, onIndex + 1, separatedInputWords.Length - onIndex - 1);

        // Find item in inventory
        PropData item = null;
        foreach (PropData prop in controller.playerInventory.items)
        {
            if (string.Equals(prop.propName, itemName, System.StringComparison.OrdinalIgnoreCase))
            {
                item = prop;
                break;
            }
        }

        if (item == null)
        {
            controller.LogStringWithReturn("You're not carrying that.");
            return;
        }

        // ── 1. Door / RoomConnection ─────────────────────────────────────────────
        RoomConnection door = controller.roomNavigation.FindConnectionByName(targetName);
        if (door != null)
        {
            if (!door.isLocked)
            {
                controller.LogStringWithReturn("That's already unlocked.");
                return;
            }
            if (door.requiredKey == item)
            {
                door.isLocked = false;
                string dirLabel = !string.IsNullOrEmpty(door.direction) ? " to the " + door.direction : "";
                controller.LogStringWithReturn("You use the " + item.propName + " to unlock the way" + dirLabel + ". (The " + item.propName + " stays with you.)");
                return;
            }
            controller.LogStringWithReturn("That doesn't seem to unlock this.");
            return;
        }

        List<PropData> roomProps = controller.roomNavigation.currentRoom.props;

        // ── 2. Locked container ──────────────────────────────────────────────────
        foreach (PropData prop in roomProps)
        {
            if (!(prop is PropContainerData container)) continue;
            if (!MatchesName(container.propName, container.description, targetName)) continue;

            if (!container.isLocked)
            {
                controller.LogStringWithReturn("That's already unlocked.");
                return;
            }
            if (container.requiredKey == item)
            {
                container.isLocked = false;
                controller.LogStringWithReturn("You use the " + item.propName + " to unlock the " + container.propName + ". (The " + item.propName + " stays with you.)");
                return;
            }
            controller.LogStringWithReturn("That doesn't seem to unlock this.");
            return;
        }

        // ── 3. Character — redirect to give ─────────────────────────────────────
        foreach (CharacterData character in controller.roomNavigation.currentRoom.characters)
        {
            if (character == null) continue;
            if (!MatchesName(character.characterName, character.description, targetName)) continue;

            controller.LogStringWithReturn("You can't use things on people. Try 'give " + item.propName + " to " + character.characterName + "' instead.");
            return;
        }

        // ── 4. Generic prop — nothing happens ────────────────────────────────────
        foreach (PropData prop in roomProps)
        {
            if (MatchesName(prop.propName, prop.description, targetName))
            {
                controller.LogStringWithReturn("Nothing happens.");
                return;
            }
        }

        // ── 5. Nothing found ─────────────────────────────────────────────────────
        controller.LogStringWithReturn("You don't see that here.");
    }

    static bool MatchesName(string name, string description, string input)
    {
        if (string.Equals(name, input, System.StringComparison.OrdinalIgnoreCase))
            return true;
        if (!string.IsNullOrEmpty(description) && description.IndexOf(input, System.StringComparison.OrdinalIgnoreCase) >= 0)
            return true;
        return false;
    }
}
