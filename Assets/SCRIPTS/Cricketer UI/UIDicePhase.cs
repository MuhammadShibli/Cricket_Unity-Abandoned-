using UnityEngine;

public class UIDicePhase : MonoBehaviour
{
   public void OnCricketerReady(CricketerSO cricketer)
    {
        Debug.Log("Cricketer selected: " + cricketer.name);
    }
}
