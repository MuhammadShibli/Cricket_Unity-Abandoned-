using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDicePhase : MonoBehaviour
{
    public GameObject rollmenu;
    public List<GameObject> dice;
    List<Rigidbody> rigidbodies = new List<Rigidbody>();
    IEnumerator checkAllAtRest = null;

    private void Awake()
    {
        rollmenu.SetActive(false);
        FindAnyObjectByType<DiceDisplayManager>().OnDiceGenerated.AddListener(OnDiceGenerated);
        rollmenu.GetComponentInChildren<Button>().onClick.AddListener(OnRollButtonClicked);
    }
    private void OnDiceGenerated(List<GameObject> arg0)
    {
        dice = arg0;
        rollmenu.SetActive(true);
    }
    private void OnRollButtonClicked()
    {
        foreach (GameObject go in dice)
        {
            var rb = go.GetComponent<Rigidbody>();
            rigidbodies.Add(rb);
            RollDice(rb); 
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
        Vector3 torque = 500* new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
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

        foreach (var rb in rigidbodies)
        {
            GetTopFace(rb);
        }
    }

    private void GetTopFace(Rigidbody rb)
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
        // You can now use this index to get the corresponding face from your DiceSO
    }

}
