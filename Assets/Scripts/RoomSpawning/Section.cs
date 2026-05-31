using Unity.VisualScripting;
using UnityEngine;

public class Section
{
    public Section[] childSections;

    public float width, height;
    public Vector2 position;

    public Section(Vector2 position, float  width, float height)
    {
        childSections = new Section[2];
        this.position = position;
        this.width = width;
        this.height = height;

        Debug.Log($"{position}, {width}, {height}");
    }
    public void SetChildSection(int index, Section section)
    {
        // If the array is not long enough
        if (childSections.Length < index)
        {
            Section[] newChildSections = new Section[index];
            for (int i = 0; i < childSections.Length; i++)
            {
                newChildSections[i] = childSections[i];
            }
            childSections = newChildSections;
        }
        // Add the new section
        childSections[index] = section;
    }
}
