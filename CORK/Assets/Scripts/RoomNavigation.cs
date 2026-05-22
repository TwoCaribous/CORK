using System.Collections.Generic;
using UnityEngine;
using CORK.Data.Rooms;

public class RoomNavigation : MonoBehaviour
{
    public RoomData currentRoom;

    Dictionary<string, List<RoomConnection>> exitDictionary = new Dictionary<string, List<RoomConnection>>();
    GameController controller;

    void Awake()
    {
        controller = GetComponent<GameController>();
    }

    public void UnpackExitsInRoom()
    {
        foreach (RoomConnection connection in currentRoom.connections)
        {
            if (connection.isHidden || connection.connectedRoom == null)
                continue;

            string directionKey = connection.direction.ToLower();
            string nameKey = connection.connectedRoom.roomName.ToLower();

            AddToExitDictionary(directionKey, connection);
            AddToExitDictionary(nameKey, connection);

            if (!string.IsNullOrEmpty(connection.doorDescription))
            {
                string exitLine = connection.hasBeenVisited
                    ? "To the " + connection.direction + " (" + connection.connectedRoom.roomName + "), " + connection.doorDescription
                    : "To the " + connection.direction + ", " + connection.doorDescription;
                controller.interactionDescriptionsInRoom.Add(exitLine);
            }
        }
    }

    void AddToExitDictionary(string key, RoomConnection connection)
    {
        if (!exitDictionary.ContainsKey(key))
            exitDictionary[key] = new List<RoomConnection>();
        exitDictionary[key].Add(connection);
    }

    public void AttemptToChangeRooms(string noun)
    {
        string key = noun.ToLower();

        List<RoomConnection> candidates = exitDictionary.ContainsKey(key)
            ? exitDictionary[key]
            : FindConnectionsByKeyword(key);

        if (candidates.Count == 0)
        {
            controller.LogStringWithReturn("There is no path to \"" + noun + "\".");
            return;
        }

        if (candidates.Count > 1)
        {
            string options = "";
            foreach (RoomConnection match in candidates)
                options += "\n  " + match.direction + " - " + match.doorDescription;
            controller.LogStringWithReturn("That could describe several exits:" + options + "\nTry a more specific description.");
            return;
        }

        RoomConnection connection = candidates[0];

        if (connection.isLocked)
        {
            string msg = !string.IsNullOrEmpty(connection.lockedMessage)
                ? connection.lockedMessage
                : "That way is locked.";
            controller.LogStringWithReturn(msg);
            return;
        }

        connection.hasBeenVisited = true;
        currentRoom = connection.connectedRoom;
        controller.LogStringWithReturn("You head to " + currentRoom.roomName + ".");
        controller.DisplayRoomText();
    }

    List<RoomConnection> FindConnectionsByKeyword(string input)
    {
        List<RoomConnection> matches = new List<RoomConnection>();
        foreach (RoomConnection connection in currentRoom.connections)
        {
            if (connection.isHidden || connection.connectedRoom == null) continue;
            if (string.IsNullOrEmpty(connection.doorDescription)) continue;

            if (connection.doorDescription.IndexOf(input, System.StringComparison.OrdinalIgnoreCase) >= 0)
                matches.Add(connection);
        }
        return matches;
    }

    public void ClearExits()
    {
        exitDictionary.Clear();
    }
}
