using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DungeonGeneration : MonoBehaviour
{
    [SerializeField] int mapWidth, mapHeight;
    [SerializeField] int maxRoomCount;

    Section sectionHead;

    List<Section> sections = new List<Section>();

    [SerializeField] GameObject roomPrefab;
    [SerializeField] int maxRoomSize;
    [SerializeField] int minRoomSize;
    [SerializeField] int roomGap;

    private void Start()
    {
        GenerateSections();

        GenerateRooms();
    }
    void GenerateSections()
    {
        Section initialSection = new Section(transform.position, mapWidth, mapHeight);
        sectionHead = initialSection;

        // Create Sections by cutting in half to a certain amount
        for (int i = 0; i < maxRoomCount; i++)
        {
            SplitSection();
        }
    }
    void SplitSection()
    {
        // Go through randomly until we come to an end
        Section cursor = sectionHead;

        bool searching = true;
        while(searching)
        {
            // Whoops
            if (cursor == null)
            {
                Debug.LogError("Cursor is null");
                break;
            }
            // Get one of the two
            Section possibleSection = cursor.childSections[Random.Range(0, 2)];

            // If its the end break else keep going
            if(possibleSection == null)
            {
                break;
            }
            else
            {
                cursor = possibleSection;
            }
        }

        bool splitHorizontally = cursor.height > cursor.width;
        float newSectionWidth = 0, newSectionHeight = 0;
        if (splitHorizontally)
        {
            // Calc their width and height for horizontal
            newSectionWidth = cursor.width;
            newSectionHeight = cursor.height / 2;

            // Create the sections

            Vector2 section1Pos = cursor.position;
            Section section1 = new Section(section1Pos, newSectionWidth, newSectionHeight);
            cursor.SetChildSection(0, section1);

            Vector2 section2Pos = new Vector2(cursor.position.x, cursor.position.y + newSectionHeight);
            Section section2 = new Section(section2Pos, newSectionWidth, newSectionHeight);
            cursor.SetChildSection(1, section2);

            sections.Add(section1);
            sections.Add(section2);
        }
        else
        {
            // Calc their width and height for vertical
            newSectionWidth = cursor.width / 2;
            newSectionHeight = cursor.height;

            // Create the sections

            Vector2 section1Pos = cursor.position;
            Section section1 = new Section(section1Pos, newSectionWidth, newSectionHeight);
            cursor.SetChildSection(0, section1);

            Vector2 section2Pos = new Vector2(cursor.position.x + newSectionWidth, cursor.position.y);
            Section section2 = new Section(section2Pos, newSectionWidth, newSectionHeight);
            cursor.SetChildSection(1, section2);

            sections.Add(section1);
            sections.Add(section2);
        }

        sections.Remove(cursor);
    }
    void GenerateRooms()
    {
        // Go through each section
        foreach (var section in sections)
        {
            // Check if the room is big enough for a room
            if (section.width < (minRoomSize + roomGap) || section.height < (minRoomSize + roomGap))
            {
                Debug.Log($"{section} is to small for a room");
                continue; // Its to small for a room
            }

            // Calculate the bounds for positioning the rooms
            float x1, y1, x2, y2;

            // Top right corner
            x1 = section.position.x + section.width - minRoomSize;
            y1 = section.position.y + section.height - minRoomSize;
            Vector2 topLeft = new Vector2 (x1, y1);

            // Bottom left corner
            x2 = section.position.x + minRoomSize;
            y2 = section.position.y + minRoomSize;
            Vector2 bottomRight = new Vector2 (x2, y2);

            // Genreate Random Pos
            Vector2 roomPos = new Vector2(
                Random.Range(topLeft.x, bottomRight.x),
                Random.Range(topLeft.y, bottomRight.y)
            );

            // Set size so it doesn't overlap
            float roomWidth = Mathf.Min(
                Mathf.Abs(roomPos.x - section.position.x - roomGap),                     // Distance from left
                Mathf.Abs((section.position.x + section.width - roomGap) - roomPos.x),   // Distance from right
                Mathf.Abs(maxRoomSize)                                         // Max room size
            );
            float roomHeight = Mathf.Min(
                Mathf.Abs(roomPos.y - section.position.y - roomGap),                     // Distance from top
                Mathf.Abs((section.position.y + section.height - roomGap) - roomPos.y),  // Distance from bottom
                Mathf.Abs(maxRoomSize)                                         // Max room size
            );
            // Adjust so it can be used for scale
            roomWidth *= 2;
            roomHeight *= 2;

            if (roomWidth < minRoomSize || roomHeight < minRoomSize)
            {
                Debug.Log($"{section} is to small for a room");
                continue;
            }

            // Create the room in the position
            GameObject room = Instantiate(roomPrefab, roomPos, Quaternion.identity);
            room.name = ($"{roomPos}, ({roomWidth}x{roomHeight})");

            SpriteRenderer spriteRenderer = room.GetComponent<SpriteRenderer>();

            room.transform.localScale = new Vector2(roomWidth, roomHeight);
        }

    }
}
