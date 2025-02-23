using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class CricketDataImporter : EditorWindow
{
    [MenuItem("Cricket/Import CSV Data")]
    public static void ImportData()
    {
        string facesPath = "Assets/csv/Cricket Dice- Metadata - Faces.csv";
        string dicePath = "Assets/csv/Cricket Dice- Metadata - Dice.csv";
        string cricketersPath = "Assets/csv/Cricket Dice- Metadata - Cricketers.csv";

        // Create an instance of CricketDataImporter to call the non-static method
        var importer= ScriptableObject.CreateInstance<CricketDataImporter>();
        
        CreateAssetFolders();
        // Import faces first
        importer.ImportFaces(facesPath);
        importer.ImportDice(dicePath);
        importer.ImportCricketers(cricketersPath);

        // Similar implementation for dice and cricketers
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void CreateAssetFolders()
    {
        string[] folders = new string[]
        {
            "Assets/ScriptableObjects",
            "Assets/ScriptableObjects/Faces",
            "Assets/ScriptableObjects/Dice",
            "Assets/ScriptableObjects/Cricketers"
        };

        foreach (string folder in folders)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                string parentFolder = Path.GetDirectoryName(folder);
                string newFolderName = Path.GetFileName(folder);
                AssetDatabase.CreateFolder(parentFolder, newFolderName);
            }
        }
    }


    void ImportFaces(string facesPath)
    {
        string[] faceLines = File.ReadAllLines(facesPath);
        for (int i = 1; i < faceLines.Length; i++) // Skip header
        {
            string[] data = faceLines[i].Split(',');
            FaceSO face = CreateInstance<FaceSO>();
            face.faceId = data[0];
            face.symbol = data[1];
            face.description = data[2];

            string assetPath = $"Assets/ScriptableObjects/Faces/{data[0]}.asset";
            AssetDatabase.CreateAsset(face, assetPath);
        }
    }

    void ImportDice(string dicePath)
    {
        // Dictionary to store created dice for later reference
        Dictionary<string, DiceSO> diceDict = new Dictionary<string, DiceSO>();

        string[] diceLines = File.ReadAllLines(dicePath);
        for (int i = 1; i < diceLines.Length; i++) // Skip header
        {
            string[] data = diceLines[i].Split(',');
            DiceSO dice = CreateInstance<DiceSO>();

            // Assign basic properties
            dice.diceId = data[0];
            dice.diceType = data[1];

            // Create faces array
            dice.faces = new FaceSO[6];

            // Assign faces by loading the previously created face assets
            for (int f = 0; f < 6; f++)
            {
                string faceId = data[f + 2]; // face IDs start at index 2
                string facePath = $"Assets/ScriptableObjects/Faces/{faceId}.asset";
                FaceSO face = AssetDatabase.LoadAssetAtPath<FaceSO>(facePath);
                if (face != null)
                {
                    dice.faces[f] = face;
                }
                else
                {
                    Debug.LogError($"Face {faceId} not found for dice {dice.diceId}");
                }
            }

            string assetPath = $"Assets/ScriptableObjects/Dice/{data[0]}.asset";
            AssetDatabase.CreateAsset(dice, assetPath);
            diceDict[dice.diceId] = dice;
        }

    }
    void ImportCricketers(string cricketersPath)
    {
        string[] cricketerLines = File.ReadAllLines(cricketersPath);
        for (int i = 1; i < cricketerLines.Length; i++) // Skip header
        {
            string[] data = cricketerLines[i].Split(',');
            CricketerSO cricketer = CreateInstance<CricketerSO>();

            // Assign basic properties
            cricketer.cricketerId = data[0];
            cricketer.cricketerName = data[1];

            // Load special dice
            cricketer.specialDice = new DiceSO[2];
            for (int d = 0; d < 2; d++)
            {
                string diceId = data[d + 2]; // special dice start at index 2
                string dicePath = $"Assets/ScriptableObjects/Dice/{diceId}.asset";
                cricketer.specialDice[d] = AssetDatabase.LoadAssetAtPath<DiceSO>(dicePath);
            }

            // Load normal dice
            cricketer.normalDice = new DiceSO[2];
            for (int d = 0; d < 2; d++)
            {
                string diceId = data[d + 4]; // normal dice start at index 4
                string dicePath = $"Assets/ScriptableObjects/Dice/{diceId}.asset";
                cricketer.normalDice[d] = AssetDatabase.LoadAssetAtPath<DiceSO>(dicePath);
            }

            // Load talent dice
            cricketer.talentDice = new DiceSO[2];
            for (int d = 0; d < 2; d++)
            {
                string diceId = data[d + 6]; // talent dice start at index 6
                string dicePath = $"Assets/ScriptableObjects/Dice/{diceId}.asset";
                cricketer.talentDice[d] = AssetDatabase.LoadAssetAtPath<DiceSO>(dicePath);
            }

            string assetPath = $"Assets/ScriptableObjects/Cricketers/{data[0]}_{data[1].Replace(" ", "_")}.asset";
            AssetDatabase.CreateAsset(cricketer, assetPath);
        }
    }

    // Editor window implementation
    [MenuItem("Cricket/Data Manager")]
    public static void ShowWindow()
    {
        GetWindow<CricketDataImporter>("Cricket Data Manager");
    }

    private void OnGUI()
    {
        GUILayout.Label("Cricket Data Importer", EditorStyles.boldLabel);

        if (GUILayout.Button("Import All Data"))
        {
            ImportData();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Import Faces Only"))
        {
            ImportFaces("Assets/csv/Cricket Dice- Metadata - Faces.csv");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            GUILayout.Space(10);
            GUILayout.Label("Faces Imported");
        }

        if (GUILayout.Button("Import Dice Only"))
        {
            ImportDice("Assets/csv/Cricket Dice- Metadata - Dice.csv");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            GUILayout.Space(10);
            GUILayout.Label("Dices Imported");
        }

        if (GUILayout.Button("Import Cricketers Only"))
        {
            ImportCricketers("Assets/csv/Cricket Dice- Metadata - Cricketers.csv");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            GUILayout.Space(10);
            GUILayout.Label("Cricketers Imported");

        }
    }
}

