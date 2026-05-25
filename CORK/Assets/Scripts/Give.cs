using System.Collections.Generic;
using UnityEngine;
using CORK.Data.Characters;
using CORK.Data.Dialogue;
using CORK.Data.Props;

/// <summary>
/// Handles the 'give' command. Transfers a carried item to a character in the room.
/// The item is consumed (removed from inventory). The character must be an EssentialCharacterData
/// and must have a matching ItemInteraction entry for the given prop.
///
/// Syntax: give [item] to [character]
///
/// Create via: Assets > Create > TextAdventure > InputActions > Give
/// Assign to GameController.inputActions[] and set keyWord to "give".
/// </summary>
[CreateAssetMenu(menuName = "TextAdventure/InputActions/Give")]
public class Give : InputAction
{
    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        // Need at least: give <item> to <character>  →  4 words minimum
        if (separatedInputWords.Length < 4)
        {
            controller.LogStringWithReturn("Give what to whom? (e.g. 'give monster to johnny')");
            return;
        }

        // Locate the "to" separator
        int toIndex = -1;
        for (int i = 1; i < separatedInputWords.Length; i++)
        {
            if (string.Equals(separatedInputWords[i], "to", System.StringComparison.OrdinalIgnoreCase))
            {
                toIndex = i;
                break;
            }
        }

        if (toIndex < 0 || toIndex == separatedInputWords.Length - 1)
        {
            controller.LogStringWithReturn("Give it to whom? (e.g. 'give monster to johnny')");
            return;
        }

        string itemName      = string.Join(" ", separatedInputWords, 1, toIndex - 1);
        string characterName = string.Join(" ", separatedInputWords, toIndex + 1, separatedInputWords.Length - toIndex - 1);

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

        // Find character in the current room
        CharacterData found = null;
        foreach (CharacterData character in controller.roomNavigation.currentRoom.characters)
        {
            if (character != null && string.Equals(character.characterName, characterName, System.StringComparison.OrdinalIgnoreCase))
            {
                found = character;
                break;
            }
        }

        if (found == null)
        {
            controller.LogStringWithReturn("There's no one like that here.");
            return;
        }

        // Only EssentialCharacterData supports item interactions
        if (!(found is EssentialCharacterData essential))
        {
            controller.LogStringWithReturn(found.characterName + " doesn't seem interested in that.");
            return;
        }

        // Find a matching ItemInteraction
        ItemInteraction match = null;
        foreach (ItemInteraction interaction in essential.itemInteractions)
        {
            if (interaction != null && interaction.expectedItem == item)
            {
                match = interaction;
                break;
            }
        }

        if (match == null || match.response == null || match.response.lines == null || match.response.lines.Count == 0)
        {
            controller.LogStringWithReturn(found.characterName + " doesn't seem interested in that.");
            return;
        }

        // Consume item and fire the response dialogue
        controller.playerInventory.RemoveItem(item);
        foreach (DialogueLine line in match.response.lines)
            controller.LogStringWithReturn(line.speakerName + ": " + line.text);
    }
}
