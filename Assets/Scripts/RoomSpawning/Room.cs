using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector2 position;
    public int width, height;

    public void InitRoom(Vector2 position, int width, int height)
    {
        this.position = position;
        this.width = width;
        this.height = height;

        SpriteRenderer sr = this.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, width, height), position);
    }
}
