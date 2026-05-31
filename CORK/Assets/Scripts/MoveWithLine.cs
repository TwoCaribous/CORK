using UnityEngine;
using CORK.Data.Rooms;
using CORK.Data;

[CreateAssetMenu(menuName = "TextAdventure/InputActions/MoveWithLine")]
public class MoveWithLine : InputAction
{
    public override void RespondToInput(GameController controller, string[] separatedInputWords)
    {
        if (separatedInputWords.Length < 2)
        {
            controller.LogStringWithReturn("Move where?");
            return;
        }

        string target = string.Join(" ", separatedInputWords, 1, separatedInputWords.Length - 1);
        
        RoomData currentRoom = controller.roomNavigation.currentRoom;
        RoomConnection targetConnection = null;
        
        foreach (var conn in currentRoom.connections)
        {
            if (conn.displayName.ToLower() == target.ToLower() ||
                (conn.hasBeenVisited && conn.connectedRoom.roomName.ToLower() == target.ToLower()) ||
                (!conn.hasBeenVisited && string.IsNullOrEmpty(conn.displayName) && conn.connectedRoom.roomName.ToLower() == target.ToLower()))
            {
                targetConnection = conn;
                break;
            }
        }
        
        if (targetConnection == null)
        {
            controller.LogStringWithReturn("You don't see a way to go '" + target + "'.");
            return;
        }
        
        // Check if trying to enter Dr. Cain's office (change "Cain" to your room name)
        bool isCainsOffice = targetConnection.connectedRoom != null && 
            (targetConnection.connectedRoom.roomName.Contains("Cain") || 
             targetConnection.connectedRoom.roomName == "343");
        
        if (isCainsOffice)
        {
            int tasksCompleted = CountLineTasks(controller);
            
            if (tasksCompleted < 3)
            {
                ShowLineMessage(controller, tasksCompleted);
                return; // Block movement
            }
        }
        
        // Handle locked doors normally
        if (targetConnection.isLocked)
        {
            string msg = string.IsNullOrEmpty(targetConnection.lockedMessage) 
                ? "It's locked." 
                : targetConnection.lockedMessage;
            controller.LogStringWithReturn(msg);
            return;
        }
        
        // Move to the room
        controller.roomNavigation.currentRoom = targetConnection.connectedRoom;
        controller.DisplayRoomText();
    }
    
    private int CountLineTasks(GameController controller)
    {
        int count = 0;
        if (controller.gameFlags.HasFlag("lineTask1Complete")) count++;
        if (controller.gameFlags.HasFlag("lineTask2Complete")) count++;
        if (controller.gameFlags.HasFlag("lineTask3Complete")) count++;
        return count;
    }
    
    private void ShowLineMessage(GameController controller, int tasksCompleted)
    {
        bool task1 = controller.gameFlags.HasFlag("lineTask1Complete");
        bool task2 = controller.gameFlags.HasFlag("lineTask2Complete");
        bool task3 = controller.gameFlags.HasFlag("lineTask3Complete");

        switch (tasksCompleted)
        {
            case 0:
                controller.LogStringWithReturn("There's a massive line stretching down the hallway. You can barely see Dr. Cain's door at the end.");
                controller.LogStringWithReturn("A student near the back groans: \"I'd do anything for a fire drill right now.\"");
                controller.LogStringWithReturn("Ahead of him, a girl checks her phone: \"Professor Garfield was supposed to call in a pizza party an hour ago. Where IS he?\"");
                controller.LogStringWithReturn("Someone further up whispers: \"...yeah, I need Hacking Henry. Is he still in 195?\"");
                break;
            case 1:
            case 2:
                LogQuestAwareLineProgress(controller, tasksCompleted, task1, task2, task3);
                break;
        }
    }

    private void LogQuestAwareLineProgress(GameController controller, int tasksCompleted, bool task1, bool task2, bool task3)
    {
        var observations = new System.Collections.Generic.List<string>();

        if (task1) observations.Add("The fire alarm cleared out about a third of the line. Students are still trickling back from the stairwell — but most of them just left.");
        if (task2) observations.Add("After Professor Garfield finally came through on the pizza party, a wave of students peeled off to go find it. You can still smell garlic bread drifting down the hall.");
        if (task3) observations.Add("Hacking Henry packed up and left the moment he got his energy drink, and half a dozen people behind him took that as their cue.");

        string chosen = observations[Random.Range(0, observations.Count)];
        controller.LogStringWithReturn(chosen);

        if (tasksCompleted == 1)
            controller.LogStringWithReturn("Still a long way to go.");
        else
            controller.LogStringWithReturn("Just a handful of stragglers remain. One more thing needs to happen.");
    }
}