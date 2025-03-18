using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiceDisplayManager : MonoBehaviour
{
    [SerializeField]
    public PhysicsMaterial dicePhysicsMaterial;
    public Material cubeMat;
    public Material spriteMat;
    public Vector3 startPosition = new Vector3(12, 65,12);
    public float faceOffset = 0.01f; // Small value to avoid Z-fighting
    public float scale = 1f;
    public float faceScale = 1f;
    public List<DiceSO> diceList = new List<DiceSO>();
    public UnityEvent<List<GameObject>> OnDiceGenerated;
    
    private List<GameObject> createdDice = new List<GameObject>();

    public void DisplayDice(List<DiceSO> diceList)
    {
        ClearDice();
        for (int i = 0; i < diceList.Count; i++)
        {
            Create3DDice(diceList[i], i);
        }
        OnDiceGenerated?.Invoke(createdDice);
    }

    public void SetDiceList(List<DiceSO> diceList)
    {
        this.diceList = diceList;
        DisplayDice(diceList);
    }

    private void Create3DDice(DiceSO dice, int index)
    {
        GameObject diceObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        SimpleDiceObject simpleDiceObject =  diceObject.AddComponent<SimpleDiceObject>();
        diceObject.name = "Dice" + index;
        diceObject.transform.localScale = Vector3.one * scale;
        diceObject.transform.position = startPosition + new Vector3(0, 0, -4 +index * (scale + 0.35f));
        diceObject.layer = LayerMask.NameToLayer("PlayerDice");     
        // Disable the default cube renderer since we're using sprites
        diceObject.GetComponent<MeshRenderer>().material=cubeMat;

        // Create faces along the normals of the cube
        Vector3[] normals = {
            Vector3.forward,
            Vector3.back,
            Vector3.up,
            Vector3.down,
            Vector3.right,
            Vector3.left
        };

        for (int i = 0; i < 6; i++)
        {
            GameObject face = new GameObject("Face" + (i+1));
            face.transform.SetParent(diceObject.transform);
            face.transform.localScale = Vector3.one * faceScale;
            

            SpriteRenderer spriteRenderer = face.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = dice.faces[i].GetSprite(dice.faces[i].spriteName);
            spriteRenderer.material = spriteMat;
            simpleDiceObject.SetFaceIndex(spriteRenderer.sprite, i+1);
            // Position each face along the normal with some distance away
            face.transform.localPosition = normals[i] *( (0.5f * scale)  + faceOffset);
            //face.transform.localRwotation = Quaternion.LookRotation(normals[i]);
            face.transform.forward = -normals[i];
            face.transform.localScale *= scale;
            // Add sorting layer and order to ensure faces render properly
            spriteRenderer.sortingOrder = 1;
        }

        var rb = diceObject.AddComponent<Rigidbody>();
        diceObject.GetComponent<BoxCollider>().material = dicePhysicsMaterial;
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
