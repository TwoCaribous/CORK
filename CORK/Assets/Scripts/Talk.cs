using System.Collections.Generic;
using UnityEngine;
using CORK.Data.Characters;
using CORK.Data.Dialogue;

[CreateAssetMenu(menuName = "TextAdventure/InputActions/Talk")]
public class Talk : InputAction
{
    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        List<CharacterData> characters = controller.roomNavigation.currentRoom.characters;

        if (separatedInputWords.Length < 2)
        {
            if (characters == null || characters.Count == 0)
            {
                controller.LogStringWithReturn("There is nobody here to talk to.");
                return;
            }

            List<string> names = new List<string>();
            foreach (CharacterData character in characters)
            {
                if (character != null) names.Add(character.characterName);
            }
            controller.LogStringWithReturn("People here: " + string.Join(", ", names) + ".");
        }
        else
        {
            string targetName = string.Join(" ", separatedInputWords, 1, separatedInputWords.Length - 1);

            CharacterData found = null;
            if (characters != null)
            {
                foreach (CharacterData character in characters)
                {
                    if (character != null && string.Equals(character.characterName, targetName, System.StringComparison.OrdinalIgnoreCase))
                    {
                        found = character;
                        break;
                    }
                }
            }

            if (found == null)
            {
                controller.LogStringWithReturn("There is nobody named \"" + targetName + "\" here.");
                return;
            }

            if (found is EssentialCharacterData essential)
            {
                DialogueEntry entry = essential.GetDialogue(controller.playerInventory);
                if (entry != null && entry.lines != null && entry.lines.Count > 0)
                {
                    foreach (DialogueLine line in entry.lines)
                        controller.LogStringWithReturn(line.speakerName + ": " + line.text);
                }
                else
                {
                    controller.LogStringWithReturn(found.characterName + " has nothing to say.");
                }
            }
            else if (found is RandomCharacterData random)
            {
                DialogueLine line = random.GetRandomAmbientLine();
                if (line != null)
                    controller.LogStringWithReturn(line.speakerName + ": " + line.text);
                else
                    controller.LogStringWithReturn(found.characterName + " says nothing.");
            }
            else
            {
                controller.LogStringWithReturn(found.characterName + " has nothing to say.");
            }
        }
    }
}
