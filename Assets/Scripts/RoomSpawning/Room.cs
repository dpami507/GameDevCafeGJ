using UnityEngine;

public class Room
{
    public Vector2 position;
    public float width, height;

    public Room(Vector2 position, float width, float height)
    {
        this.position = position;
        this.width = width;
        this.height = height;
    }
}
