using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIDicePhase : MonoBehaviour
{
    public UnityEvent OnPlayerRollComplete = new UnityEvent();
    public TMP_Text rollButtonText;
    public GameObject rollmenu;
    public YourDicePanel[] yourDicePanels;
    public List<GameObject> dice;
    List<Rigidbody> rigidbodies = new List<Rigidbody>();
    IEnumerator checkAllAtRest = null;
    int[] frozenDice = new int[6];
    int numOfRolls = 3;

    private void Awake()
    {
        rollmenu.SetActive(false);
        FindAnyObjectByType<DiceDisplayManager>().OnDiceGenerated.AddListener(OnDiceGenerated);
    }
    private void OnDiceGenerated(List<GameObject> arg0)
    {
        dice = arg0;
        rollmenu.SetActive(true);
    }
    public void OnRollButtonClicked()
    {
        rigidbodies = new List<Rigidbody>();
        if(checkAllAtRest != null)
        {
            StopCoroutine(checkAllAtRest);
        }


        if (numOfRolls == 0)
        {
            rollButtonText.text = "Done";
            return;
        }

        numOfRolls--;
        rollButtonText.text = numOfRolls > 0 ? $"Roll x {numOfRolls}" : "Done";
        for (int i = 0; i < dice.Count; i++)
        {
            {
                var rb = dice[i].GetComponent<Rigidbody>();
                rigidbodies.Add(rb);
                if (frozenDice[i] != 1)
                    RollDice(rb);
            }


        }
        checkAllAtRest = CheckRollFinished();
        StartCoroutine(checkAllAtRest);

    }

    void RollDice(Rigidbody rb)
    {
           
        // Apply linear force
        Vector3 forceDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), 1, UnityEngine.Random.Range(-1f, 1f)).normalized;
        float forceMagnitude = 100f;
        rb.AddForce(forceDirection * forceMagnitude);

        // Apply torque
        Vector3 torque = 500 * new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
        rb.AddTorque(torque);

        // Apply explosive force
        Vector3 explosionPosition = rb.transform.position + Vector3.down * 0.5f;
        float explosionForce = 100f;
        float explosionRadius = 1f;
        rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);

    }

    IEnumerator CheckRollFinished()
    {
          

        bool allAtRest = false;
        while (!allAtRest)
        {
            foreach (var rb in rigidbodies)
            {
                if (rb.IsSleeping())
                {
                    allAtRest = true;
                }
                else
                {
                    allAtRest = false;
                    break;
                }
            }
            yield return new WaitForSeconds(1f);
        }
        //all are at rest now lets get their top faces

        for (int i = 0; i < 6; i++)
        {
            int topfaceIndex = GetTopFace(rigidbodies[i], i);
        }
        OnPlayerRollComplete?.Invoke();
        checkAllAtRest = null;
    }
    

    private int GetTopFace(Rigidbody rb,int index)
    {
        // The possible face directions in local space
        Vector3[] directions = {
          // Top (Y+)
        Vector3.forward, // Front (Z+)
        Vector3.back,   // Back (Z-)
        Vector3.up,    
        Vector3.down,  // Bottom (Y-)
        Vector3.right,  // Right (X+)
        Vector3.left    // Left (X-)
    };

        // Find which direction is most aligned with world up
        float maxAlignment = float.MinValue;
        int topFaceIndex = -1;

        for (int i = 0; i < directions.Length; i++)
        {
            // Convert the local direction to world space using the rigidbody's rotation
            Vector3 worldDir = rb.rotation * directions[i];

            // Compare with world up using dot product
            float alignment = Vector3.Dot(worldDir, Vector3.up);

            if (alignment > maxAlignment)
            {
                maxAlignment = alignment;
                topFaceIndex = i;
            }
        }
        SimpleDiceObject sdo = rb.GetComponent<SimpleDiceObject>();

        Debug.Log($"Top face index: {topFaceIndex} which is {sdo.GetSpriteAtIndex(topFaceIndex).ToString()} ");
        AddtoYourDicePanel(index, sdo, topFaceIndex);
        return topFaceIndex;
        // You can now use this index to get the corresponding face from your DiceSO
       
    }

    private void AddtoYourDicePanel(int index, SimpleDiceObject sdo, int topFaceIndex)
    {
        yourDicePanels[index].Image.sprite = sdo.GetSpriteAtIndex(topFaceIndex);
        yourDicePanels[index].text.text = sdo.GetSpriteAtIndex(topFaceIndex).name;
    }

    public void FreezeDice(int i)
    {
        if (frozenDice[i] == 1)
        {
            frozenDice[i] = 0;
            yourDicePanels[i].Image.color = Color.white;
            Debug.Log($"unfroze {i}");
        }
        else
        {
            frozenDice[i] = 1;
            yourDicePanels[i].Image.color = Color.yellow;
            Debug.Log($"froze {i}");
        }
    }
}

[Serializable]
public struct YourDicePanel
{
    public Image Image;
    public TMP_Text text;
}