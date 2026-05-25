using System.Collections.Generic;
using UnityEngine;
using CORK.Data.Props;

/// <summary>
/// Handles the 'take' command. Searches the current room's props and the contents
/// of any open containers for a matching prop, then moves it to the player's inventory.
///
/// Create via: Assets > Create > TextAdventure > InputActions > Take
/// Assign to GameController.inputActions[] and set keyWord to "take".
/// </summary>
[CreateAssetMenu(menuName = "TextAdventure/InputActions/Take")]
public class Take : InputAction
{
    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        if (separatedInputWords.Length < 2)
        {
            controller.LogStringWithReturn("Take what?");
            return;
        }

        string input = string.Join(" ", separatedInputWords, 1, separatedInputWords.Length - 1);
        List<PropData> roomProps = controller.roomNavigation.currentRoom.props;

        PropContainerData sourceContainer;
        PropData found = FindAccessibleProp(roomProps, input, out sourceContainer);

        if (found == null)
        {
            controller.LogStringWithReturn("You don't see that here.");
            return;
        }

        if (!found.canBeTaken)
        {
            string msg = !string.IsNullOrEmpty(found.cantTakeMessage)
                ? found.cantTakeMessage
                : "You can't take that.";
            controller.LogStringWithReturn(msg);
            return;
        }

        string takeMsg = !string.IsNullOrEmpty(found.takeMessage)
            ? found.takeMessage
            : "You take the " + found.propName + ".";

        // Prop is inside an open container — remove from container and add to inventory directly.
        if (sourceContainer != null)
        {
            sourceContainer.containedProps.Remove(found);
            controller.playerInventory.AddItem(found);
            controller.LogStringWithReturn(takeMsg);
            return;
        }

        // Prop is a room-level prop — delegate to PropPickupSystem.
        if (controller.TryTakeFromRoom(found))
            controller.LogStringWithReturn(takeMsg);
        else
            controller.LogStringWithReturn(!string.IsNullOrEmpty(found.cantTakeMessage) ? found.cantTakeMessage : "You can't take that.");
    }

    /// <summary>
    /// Searches room-level props first, then the contents of any open containers.
    /// Sets sourceContainer to the container the prop was found in, or null for room-level props.
    /// </summary>
    static PropData FindAccessibleProp(List<PropData> roomProps, string input, out PropContainerData sourceContainer)
    {
        sourceContainer = null;
        if (roomProps == null) return null;

        // ── Exact name: room level ────────────────────────────────────────────────
        foreach (PropData prop in roomProps)
        {
            if (prop != null && string.Equals(prop.propName, input, System.StringComparison.OrdinalIgnoreCase))
                return prop;
        }

        // ── Exact name: open containers ───────────────────────────────────────────
        foreach (PropData prop in roomProps)
        {
            if (prop is PropContainerData container && container.isOpen && container.containedProps != null)
            {
                foreach (PropData contained in container.containedProps)
                {
                    if (contained != null && string.Equals(contained.propName, input, System.StringComparison.OrdinalIgnoreCase))
                    {
                        sourceContainer = container;
                        return contained;
                    }
                }
            }
        }

        // ── Description match: room level ─────────────────────────────────────────
        List<PropData> roomMatches = new List<PropData>();
        foreach (PropData prop in roomProps)
        {
            if (prop == null || string.IsNullOrEmpty(prop.description)) continue;
            if (prop.description.IndexOf(input, System.StringComparison.OrdinalIgnoreCase) >= 0)
                roomMatches.Add(prop);
        }
        if (roomMatches.Count == 1) return roomMatches[0];
        if (roomMatches.Count > 1)
        {
            // Signal ambiguity — caller will receive null; we can't return a message here.
            // Ambiguous room props: just return null and let the caller say "be more specific".
            return null;
        }

        // ── Description match: open containers ────────────────────────────────────
        List<(PropData prop, PropContainerData container)> containerMatches
            = new List<(PropData, PropContainerData)>();

        foreach (PropData prop in roomProps)
        {
            if (prop is PropContainerData container && container.isOpen && container.containedProps != null)
            {
                foreach (PropData contained in container.containedProps)
                {
                    if (contained == null || string.IsNullOrEmpty(contained.description)) continue;
                    if (contained.description.IndexOf(input, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        containerMatches.Add((contained, container));
                }
            }
        }

        if (containerMatches.Count == 1)
        {
            sourceContainer = containerMatches[0].container;
            return containerMatches[0].prop;
        }

        return null;
    }
}
