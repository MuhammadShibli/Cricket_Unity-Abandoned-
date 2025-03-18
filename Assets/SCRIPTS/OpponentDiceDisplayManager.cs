using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public class OpponentDiceDisplayManager : MonoBehaviour
{
    [Header("Cricketer Data")]
    public CricketerSO opponentCricketer; // Assign the opponent's cricketer

    [Header("Dice Pool")]
    public DicePoolSO opponentDicePool;   // Pool used to generate dice from the cricketer

    [Header("Physics & Materials")]
    public PhysicsMaterial opponentDicePhysicsMaterial;
    public Material opponentCubeMat;     // Black material for the opponent dice
    public Material opponentSpriteMat;   // Material for dice faces

    [Header("Positioning & Scale")]
    public Vector3 startPosition = new Vector3(12, 65, -12); // Opponent dice start position
    public float faceOffset = 0.01f;    // Small offset to avoid z-fighting
    public float scale = 1f;
    public float faceScale = 1f;

    [Header("Dice Data & Events")]
    // We build the dice list from the opponentCricketer
    public UnityEvent<List<GameObject>> OnOpponentDiceGenerated;

    // Number of automatic rerolls (after initial roll)
    public int numOfRerolls = 2;

    private List<GameObject> createdDice = new List<GameObject>();
    // Array to track which dice are frozen (true if frozen)
    private bool[] frozenDice;
    IEnumerator autoRollCoroutine = null;
    // Call this to generate the opponent dice from the opponentCricketer.
    public YourDicePanel[] yourDicePanels = new YourDicePanel[6];

    private void Awake()
    {
        // Find and listen to the player's UIDicePhase for roll completion
        UIDicePhase playerDicePhase = FindFirstObjectByType<UIDicePhase>();
        if (playerDicePhase != null)
        {
            // Subscribe to the player's roll completion
            playerDicePhase.OnPlayerRollComplete.AddListener(OnPlayerRollComplete);
        }
    }

    private void OnPlayerRollComplete()
    {
        Debug.Log("Player roll complete, starting opponent roll");
       if (autoRollCoroutine == null)
        {
            autoRollCoroutine = AutomateOpponentRoll();
            StartCoroutine(autoRollCoroutine);
        }
        
    }

    public void GenerateOpponentDice()
    {
        if (opponentCricketer == null)
        {
            Debug.LogError("Opponent Cricketer is not assigned.");
            return;
        }
        if (opponentDicePool == null)
        {
            Debug.LogError("Opponent Dice Pool is not assigned.");
            return;
        }

        opponentDicePool.ResetDicePool();

        // Populate the pool:
        foreach (DiceSO d in opponentCricketer.normalDice)
        {
            for (int i = 0; i < 3; i++)
                opponentDicePool.AddDice(d);
        }
        foreach (DiceSO d in opponentCricketer.specialDice)
        {
            for (int i = 0; i < 2; i++)
                opponentDicePool.AddDice(d);
        }
        foreach (DiceSO d in opponentCricketer.talentDice)
        {
            opponentDicePool.AddDice(d);
        }

        // Draw 6 dice from the pool.
        List<DiceSO> diceList = opponentDicePool.DrawRandomDice(6);
        DisplayDice(diceList);
        // Initialize freeze state tracking.
        frozenDice = new bool[createdDice.Count];
        // Don't automatically roll - wait for player roll
    }

    public void DisplayDice(List<DiceSO> diceList)
    {
        ClearDice();

        for (int i = 0; i < diceList.Count; i++)
        {
            Create3DDice(diceList[i], i);
        }

        OnOpponentDiceGenerated?.Invoke(createdDice);
    }

    // This method creates a single 3D dice using a primitive cube with opponent-specific modifications.
    private GameObject Create3DDice(DiceSO dice, int index)
    {
        GameObject diceObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        SimpleDiceObject simpleDiceObject = diceObject.AddComponent<SimpleDiceObject>();
        diceObject.name = "OpponentDice" + index;
        diceObject.transform.localScale = Vector3.one * scale;
        diceObject.transform.position = startPosition + new Vector3(-4, 0, (-4 + index * (scale + 0.35f)));

        // Set the dice layer so they don't collide with player dice.
        diceObject.layer = LayerMask.NameToLayer("OpponentDice");

        // Use the opponent cube material (black appearance)
        MeshRenderer renderer = diceObject.GetComponent<MeshRenderer>();
        renderer.material = opponentCubeMat;

        // Define face normals in fixed order.
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
            GameObject face = new GameObject("Face" + (i + 1));
            face.transform.SetParent(diceObject.transform);
            face.transform.localScale = Vector3.one * faceScale;

            SpriteRenderer spriteRenderer = face.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = dice.faces[i].GetSprite(dice.faces[i].spriteName);
            spriteRenderer.material = opponentSpriteMat;
            simpleDiceObject.SetFaceIndex(spriteRenderer.sprite, i + 1);  // Changed this line to match DiceDisplayManager
                                                                          // Now using i + 1 instead of just i

            face.transform.localPosition = normals[i] * (0.5f * scale + faceOffset);
            face.transform.forward = -normals[i];
            face.transform.localScale *= scale;
            spriteRenderer.sortingOrder = 1;
        }

        Rigidbody rb = diceObject.AddComponent<Rigidbody>();
        BoxCollider bc = diceObject.GetComponent<BoxCollider>();
        bc.material = opponentDicePhysicsMaterial;

        createdDice.Add(diceObject);
        Debug.Log($"Created opponent dice object {diceObject.name} with scale: {scale} at position: {diceObject.transform.position}");
        return diceObject;
    }

    public void ClearDice()
    {
        foreach (GameObject dice in createdDice)
        {
            if (dice != null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnityEditor.Undo.DestroyObjectImmediate(dice);
                }
                else
                {
                    Destroy(dice);
                }
#else
            Destroy(dice);
#endif
            }
        }
        createdDice.Clear();
    }

    private IEnumerator AutomateOpponentRoll()
    {
        // Make sure we have dice and frozen state array
        if (createdDice == null || createdDice.Count == 0)
        {
            Debug.LogError("No dice created for opponent roll");
            yield break;
        }

        // Initial roll - roll all dice first
        Debug.Log("Opponent initial roll");
        foreach (GameObject diceObj in createdDice)
        {
            Rigidbody rb = diceObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                RollDice(rb);
            }
        }

        // Wait for initial roll to settle
        yield return StartCoroutine(CheckRollFinished());
        yield return new WaitForSeconds(0.2f);

        // Perform rerolls
        for (int reroll = 0; reroll < numOfRerolls; reroll++)
        {
            Debug.Log($"Opponent reroll {reroll + 1}");

            // First make freeze decisions
            for (int i = 0; i < createdDice.Count; i++)
            {
                if (!frozenDice[i])
                {
                    // 50% chance to freeze each die
                    bool shouldFreeze = Random.value > 0.8f;
                    if (shouldFreeze)
                    {
                        frozenDice[i] = true;
                        // Change color to indicate frozen state
                        MeshRenderer renderer = createdDice[i].GetComponent<MeshRenderer>();
                        if (renderer != null)
                        {
                            // Create a new material instance to avoid affecting other dice
                            renderer.material = new Material(opponentCubeMat);
                            renderer.material.color = Color.yellow;
                        }
                        Debug.Log($"Opponent froze dice {i}");
                    }
                }
            }

            // Wait a bit after freezing decisions
            yield return new WaitForSeconds(0.5f);

            // Roll unfrozen dice
            bool anyDiceRolled = false;
            foreach (GameObject diceObj in createdDice)
            {
                int index = createdDice.IndexOf(diceObj);
                if (!frozenDice[index])
                {
                    Rigidbody rb = diceObj.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        RollDice(rb);
                        anyDiceRolled = true;
                    }
                }
            }

            // If all dice are frozen, end early
            if (!anyDiceRolled)
            {
                Debug.Log("All opponent dice frozen, ending rerolls early");
                break;
            }

            // Wait for dice to settle
            yield return StartCoroutine(CheckRollFinished());
            yield return new WaitForSeconds(1f);
        }

        Debug.Log("Opponent dice rolls complete");
    }

    private void RollDice(Rigidbody rb)
    {
        if (rb == null) return;

        // Reset the dice state
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Match player dice force values exactly
        Vector3 forceDirection = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)).normalized;
        float forceMagnitude = 1000;  // Matched to player value
        rb.AddForce(forceDirection * forceMagnitude);

        // Match player torque
        Vector3 torque = 500f * new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rb.AddTorque(torque);

        // Match player explosion force values
        Vector3 explosionPosition = rb.transform.position + Vector3.down * 0.5f;
        float explosionForce = 300f;  // Matched to player value
        float explosionRadius = 2f;    // Matched to player value
        rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
    }

    private IEnumerator CheckRollFinished()
    {
        bool allAtRest = false;
        while (!allAtRest)
        {
            allAtRest = true;
            foreach (GameObject diceObj in createdDice)
            {
                Rigidbody rb = diceObj.GetComponent<Rigidbody>();
                if (!rb.IsSleeping())
                {
                    allAtRest = false;
                    break;
                }
            }
            yield return new WaitForSeconds(1f);
        } // Add this part to actually check the faces when dice stop
        for (int i = 0; i < createdDice.Count; i++)
        {
            Rigidbody rb = createdDice[i].GetComponent<Rigidbody>();
            GetTopFace(rb, i);
        }
        yield return null;
    }

    private int GetTopFace(Rigidbody rb, int index)
    {
        // The possible face directions in local space
        Vector3[] directions = {
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
                SimpleDiceObject sdo = rb.GetComponent<SimpleDiceObject>();
                if (sdo != null)
                {
                    yourDicePanels[i].Image.sprite = sdo.GetSpriteAtIndex(topFaceIndex);
                    yourDicePanels[i].text.text = sdo.GetSpriteAtIndex(topFaceIndex).name;  
                }
            }
        }

       

        return topFaceIndex;
    }
}
