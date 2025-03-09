using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using System.Linq;
using System;
public class CricketerCell : MonoBehaviour
{
    [Header("UI References")]
    public TMPro.TextMeshProUGUI cricketerName;
    public Image image;
    public Button selectButton;
    public CricketerSO cricketer;
    public UnityEvent<CricketerSO> onCricketerSelected;
  
    private void Awake()
    {
        // Try to find references if they're not assigned
        if (cricketerName == null)
            cricketerName = GetComponentInChildren<TMPro.TextMeshProUGUI>();

        if (image == null)
            image = GetComponent<Image>();

        if (selectButton == null)
            selectButton = GetComponent<Button>();
    }
    public CricketerCell(CricketerSO cricketer)
    {
        Initialise(cricketer);
    }
    public void Initialise(CricketerSO cricketer)
    {
        this.cricketer = cricketer;

        if (cricketerName != null)
        {
            cricketerName.text = cricketer.cricketerName;
        }
        else
        {
            Debug.LogWarning("cricketerName is not assigned.");
        }

        if (image != null)
        {
            image.sprite = cricketer.Image;
        }
        else
        {
            Debug.LogWarning("image is not assigned.");
        }

        if (selectButton != null)
        {
            selectButton.onClick.AddListener(SelectionComplete); 
        }
        else
        {
            Debug.LogWarning("selectButton is not assigned.");
        }
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif    
    }

    void SelectionComplete()
    {
        Debug.Log(" from cell Cricketer selected: " + cricketer.cricketerName);
        onCricketerSelected?.Invoke(cricketer);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CricketerCell))]
    public class CricketerCellEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Debug Load Random Cricketer"))
            {
                var cell = (CricketerCell)target;
                var guids = AssetDatabase.FindAssets("t:CricketerSO");
                if (guids.Length > 0)
                {
                    var randomGuid = guids[UnityEngine.Random.Range(0, guids.Length)];
                    var path = AssetDatabase.GUIDToAssetPath(randomGuid);
                    var randomCricketer = AssetDatabase.LoadAssetAtPath<CricketerSO>(path);

                    if (randomCricketer != null)
                    {
                        cell.Initialise(randomCricketer);
                    }
                }
                else
                {
                    Debug.LogWarning("No CricketerSO assets found in the project!");
                }
            }
        }
    }
#endif

}
