using System.Collections.Generic;
using UnityEngine;

public class DiceDisplayManager : MonoBehaviour
{
    public Vector3 startPosition = new Vector3(0, 0, 0);
    public Vector3 spacing = new Vector3(1, 0, 0);
    public float scale = 1f;
    public float faceScale = 0.2f;
    public List<DiceSO> diceList = new List<DiceSO>(); // Add this line

    private List<GameObject> createdDice = new List<GameObject>();

    public void DisplayDice(List<DiceSO> diceList)
    {
        ClearDice();
        for (int i = 0; i < diceList.Count; i++)
        {
            Create3DDice(diceList[i], i);
        }
    }

    private void Create3DDice(DiceSO dice, int index)
    {
        GameObject diceObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        diceObject.transform.localScale = Vector3.one * scale;
        diceObject.transform.position = startPosition + (spacing * index);

        // Disable the default cube renderer since we're using sprites
        diceObject.GetComponent<MeshRenderer>().enabled = false;

        for (int i = 0; i < 6; i++)
        {
            GameObject face = new GameObject("Face" + (i + 1));
            face.transform.SetParent(diceObject.transform);
            face.transform.localScale = Vector3.one * DiceSO.FACE_SCALE;

            SpriteRenderer spriteRenderer = face.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = dice.faces[i].GetSprite(dice.faces[i].spriteName);
          
            // Position and rotate each face properly
            switch (i)
            {
                case 0: // Front
                    face.transform.localPosition = new Vector3(0, 0, DiceSO.FACE_OFFSET);
                    face.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    face.transform.localScale = Vector3.one * faceScale;
                    break;
                case 1: // Back
                    face.transform.localPosition = new Vector3(0, 0, -DiceSO.FACE_OFFSET);
                    face.transform.localRotation = Quaternion.Euler(0, 180, 0); 
                    face.transform.localScale = Vector3.one * faceScale;
                    break;
                case 2: // Top
                    face.transform.localPosition = new Vector3(0, DiceSO.FACE_OFFSET, 0);
                    face.transform.localRotation = Quaternion.Euler(90, 0, 0);
                    face.transform.localScale = Vector3.one * faceScale;
                    break;
                case 3: // Bottom
                    face.transform.localPosition = new Vector3(0, -DiceSO.FACE_OFFSET, 0);
                    face.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                    face.transform.localScale = Vector3.one * faceScale;
                    break;
                case 4: // Right
                    face.transform.localPosition = new Vector3(DiceSO.FACE_OFFSET, 0, 0);
                    face.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    face.transform.localScale = Vector3.one * faceScale;
                    break;
                case 5: // Left
                    face.transform.localPosition = new Vector3(-DiceSO.FACE_OFFSET, 0, 0);
                    face.transform.localRotation = Quaternion.Euler(0, -90, 0);
                    face.transform.localScale = Vector3.one * faceScale;
                    break;
            }

            // Add sorting layer and order to ensure faces render properly
            spriteRenderer.sortingOrder = 1;
        }

        createdDice.Add(diceObject);
        Debug.Log($"Created dice object with scale: {scale} at position: {diceObject.transform.position}");
    }

    public void ClearDice()
    {
        foreach (GameObject dice in createdDice)
        {
            Destroy(dice);
        }
        createdDice.Clear();
    }
}
