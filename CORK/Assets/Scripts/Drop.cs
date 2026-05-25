using UnityEngine;
using CORK.Data.Props;

/// <summary>
/// Handles the 'drop' command. Removes a prop from the player's inventory and
/// places it on the floor of the current room.
///
/// Dropped props are tracked in GameController.droppedProps so DisplayRoomText
/// can prefix them with "On the floor: ". Dropped items are always takeable,
/// regardless of their original canBeTaken setting.
///
/// Create via: Assets > Create > TextAdventure > InputActions > Drop
/// Assign to GameController.inputActions[] and set keyWord to "drop".
/// </summary>
[CreateAssetMenu(menuName = "TextAdventure/InputActions/Drop")]
public class Drop : InputAction
{
    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        if (separatedInputWords.Length < 2)
        {
            controller.LogStringWithReturn("Drop what?");
            return;
        }

        string input = string.Join(" ", separatedInputWords, 1, separatedInputWords.Length - 1);

        PropData found = null;
        foreach (PropData prop in controller.playerInventory.items)
        {
            if (string.Equals(prop.propName, input, System.StringComparison.OrdinalIgnoreCase))
            {
                found = prop;
                break;
            }
        }

        if (found == null)
        {
            controller.LogStringWithReturn("You're not carrying that.");
            return;
        }

        // Remove from inventory, mark always takeable, add to room floor
        controller.playerInventory.RemoveItem(found);
        found.canBeTaken = true;
        found.hasBeenDiscovered = true;
        controller.roomNavigation.currentRoom.props.Add(found);
        controller.droppedProps.Add(found);

        controller.LogStringWithReturn("You drop the " + found.propName + ".");
    }
}
