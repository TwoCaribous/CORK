using UnityEngine;

[CreateAssetMenu(menuName = "TextAdventure/InputActions/Pull")]
public class PullCommand : InputAction
{
    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        if (separatedInputWords.Length < 2)
        {
            controller.LogStringWithReturn("Pull what?");
            return;
        }

        string target = string.Join(" ", separatedInputWords, 1, separatedInputWords.Length - 1);
        
        // Check for fire alarm in current room
        bool foundAlarm = false;
        foreach (var prop in controller.roomNavigation.currentRoom.props)
        {
            if (prop != null && prop.propName.ToLower() == target.ToLower())
            {
                foundAlarm = true;
                
                if (prop.propName.ToLower() == "fire alarm")
                {
                    if (controller.gameFlags.HasFlag("alarmPulled"))
                    {
                        controller.LogStringWithReturn("The alarm is already ringing.");
                        return;
                    }
                    
                    controller.gameFlags.SetFlag("alarmPulled");
                    controller.gameFlags.SetFlag("lineTask1Complete");
                    controller.LogStringWithReturn("You pull the fire alarm. Sirens blare! Students evacuate. The line outside Dr. Cain's office is now shorter.");
                    return;
                }
                else
                {
                    controller.LogStringWithReturn("You can't pull the " + prop.propName + ".");
                    return;
                }
            }
        }
        
        controller.LogStringWithReturn("You don't see a " + target + " here.");
    }
}