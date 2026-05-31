using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DungeonGeneration : MonoBehaviour
{
    [SerializeField] int mapWidth, mapHeight;
    [SerializeField] int maxRoomCount;

    Section sectionHead;

    List<Section> sections = new List<Section>();

    private void Start()
    {
        Generate();
    }
    void Generate()
    {
        Section initialSection = new Section(transform.position, mapWidth, mapHeight);
        sectionHead = initialSection;

        // Create Sections by cutting in half to a certain amount
        for (int i = 0; i < maxRoomCount; i++)
        {
            Split();
        }

        foreach (var section in sections)
        {
            GameObject visual = new GameObject($"{section.position}, {section.width}x{section.height}");
            SpriteRenderer spriteRenderer = visual.AddComponent<SpriteRenderer>();
            Sprite sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(Vector2.zero, Vector2.one), Vector2.zero, 1);

            spriteRenderer.sprite = sprite;
            spriteRenderer.color = Random.ColorHSV();
            visual.transform.localScale = new Vector2(section.width, section.height);
            visual.transform.localPosition = section.position;
        }
    }
    void Split()
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

        bool splitHorizontally = Random.Range(0, 2) != 0;
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
}
