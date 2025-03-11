using UnityEngine;

public class SimpleDiceObject : MonoBehaviour
{
    Sprite[] faces = new Sprite[6];
    public void SetFaceIndex(Sprite face, int index)
    {
        faces[index-1] = face;
    }

    public Sprite GetSpriteAtIndex(int index)
    {
        return faces[index];
    }
}
