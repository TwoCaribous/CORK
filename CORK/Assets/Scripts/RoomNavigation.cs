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

            if (connection.hasBeenVisited)
            {
                AddToExitDictionary(connection.connectedRoom.roomName.ToLower(), connection);
            }
            else
            {
                // Use displayName as the navigation key pre-visit; fall back to roomName if none is set.
                string preVisitKey = !string.IsNullOrEmpty(connection.displayName)
                    ? connection.displayName.ToLower()
                    : connection.connectedRoom.roomName.ToLower();
                AddToExitDictionary(preVisitKey, connection);
            }

            string exitLine;
            if (connection.hasBeenVisited)
            {
                exitLine = "To the " + connection.direction;
                string segment = "(" + connection.connectedRoom.roomName + ") ";
                if (!string.IsNullOrEmpty(connection.doorDescription))
                    segment += connection.doorDescription;
                exitLine += ", " + segment.TrimEnd();
            }
            else
            {
                exitLine = "To the " + connection.direction;
                string segment = "";
                if (!string.IsNullOrEmpty(connection.displayName))
                    segment += "(" + connection.displayName + ") ";
                if (!string.IsNullOrEmpty(connection.doorDescription))
                    segment += connection.doorDescription;
                if (!string.IsNullOrEmpty(segment))
                    exitLine += ", " + segment.TrimEnd();
            }
            controller.interactionDescriptionsInRoom.Add(exitLine);
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
            : new List<RoomConnection>();

        if (candidates.Count == 0)
        {
            controller.LogStringWithReturn("There is no path to \"" + noun + "\".");
            return;
        }

        if (candidates.Count > 1)
        {
            string options = "";
            foreach (RoomConnection match in candidates)
                options += "\n  " + match.direction;
            controller.LogStringWithReturn("Multiple exits match \"" + noun + "\":" + options + "\nTry using the direction instead.");
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
        controller.LogStringWithReturn("<color=#FFD700>You head to " + currentRoom.roomName + ".</color>");
        controller.LogRawStringWithReturn("");
        controller.DisplayRoomText();
    }

    public void ClearExits()
    {
        exitDictionary.Clear();
    }

    /// <summary>
    /// Finds a connection in the current room by direction, displayName, roomName, or doorDescription.
    /// Hidden connections are excluded. Returns null if nothing matches.
    /// </summary>
    public RoomConnection FindConnectionByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        foreach (RoomConnection conn in currentRoom.connections)
        {
            if (conn == null || conn.connectedRoom == null || conn.isHidden) continue;
            if (string.Equals(conn.direction, name, System.StringComparison.OrdinalIgnoreCase)) return conn;
            if (!string.IsNullOrEmpty(conn.displayName) && string.Equals(conn.displayName, name, System.StringComparison.OrdinalIgnoreCase)) return conn;
            if (string.Equals(conn.connectedRoom.roomName, name, System.StringComparison.OrdinalIgnoreCase)) return conn;
            if (!string.IsNullOrEmpty(conn.doorDescription) && conn.doorDescription.IndexOf(name, System.StringComparison.OrdinalIgnoreCase) >= 0) return conn;
        }
        return null;
    }
}
