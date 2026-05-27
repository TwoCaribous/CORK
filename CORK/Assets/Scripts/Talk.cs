using System.Collections.Generic;
using UnityEngine;
using CORK.Data.Characters;
using CORK.Data.Dialogue;

[CreateAssetMenu(menuName = "TextAdventure/InputActions/Talk")]
public class Talk : InputAction
{
    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        if (separatedInputWords.Length < 2)
        {
            controller.LogStringWithReturn("Talk to whom? Look around to see who's here.");
            return;
        }

        string input = string.Join(" ", separatedInputWords, 1, separatedInputWords.Length - 1);
        List<CharacterData> characters = controller.roomNavigation.currentRoom.characters;

        CharacterData found = null;
        foreach (CharacterData character in characters)
        {
            if (character != null && string.Equals(character.characterName, input, System.StringComparison.OrdinalIgnoreCase))
            { found = character; break; }
        }

        if (found == null)
        {
            List<CharacterData> matches = FindCharactersByDescription(characters, input);
            if (matches.Count == 1)
                found = matches[0];
            else if (matches.Count > 1)
            {
                controller.LogStringWithReturn("That could describe several people here. Try being more specific.");
                return;
            }
        }

        if (found == null)
        {
            controller.LogStringWithReturn("You don't see anyone like that here.");
            return;
        }

        bool newMeeting = !found.hasBeenMet;
        found.hasBeenMet = true;

        if (found is EssentialCharacterData essential)
        {
            DialogueEntry entry = essential.GetDialogue(controller.playerInventory, controller.gameFlags);
            if (entry != null && entry.lines != null && entry.lines.Count > 0)
            {
                string dialogueText = string.Join("\n", entry.lines.ConvertAll(line => line.speakerName + ": " + line.text).ToArray());
                controller.LogStringWithReturn(dialogueText);
            }
            else
            {
                controller.LogStringWithReturn("They don't seem to want to talk.");
            }
        }
        else if (found is RandomCharacterData random)
        {
            DialogueLine line = random.GetRandomAmbientLine();
            if (line != null)
                controller.LogStringWithReturn(line.speakerName + ": " + line.text);
            else
                controller.LogStringWithReturn("They say nothing.");
        }
        else
        {
            controller.LogStringWithReturn("They don't seem to want to talk.");
        }

        if (newMeeting)
        {
            controller.LogRawStringWithReturn("");
            controller.LogStringWithReturn("You learn their name is: " + found.characterName + ".");
        }
    }

    static List<CharacterData> FindCharactersByDescription(List<CharacterData> characters, string input)
    {
        List<CharacterData> matches = new List<CharacterData>();
        if (characters == null) return matches;

        foreach (CharacterData character in characters)
        {
            if (character == null || string.IsNullOrEmpty(character.description)) continue;
            if (character.description.IndexOf(input, System.StringComparison.OrdinalIgnoreCase) >= 0)
                matches.Add(character);
        }

        return matches;

        return null;
    }
}
