using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.U2D.Sprites;
using Unity.VisualScripting;

public class SpriteManager
{
   
    public class SpriteData
    {
        public int id;
        public string name;
        public Sprite sprite;
    }
    private const string SPRITE_PATH = "Assets/Resources/cricket_SS.png";
    private static List<SpriteData> spritesData;
    private static bool initialized = false;
    public static void Initialize()
    {
        if(initialized)
        {
            return;
        }

        Sprite[] cricketSprites = AssetDatabase.LoadAllAssetsAtPath(SPRITE_PATH).OfType<Sprite>().ToArray();
        if (cricketSprites == null || cricketSprites.Length == 0)
        {
            Debug.LogError($"No sprites found at path: {SPRITE_PATH}");
        }
        string[] spriteNames = cricketSprites.Select(s => s.name).ToArray();
        spritesData = new List<SpriteData>();

        for(int i =0; i < spriteNames.Length; i++)
        {
            SpriteData spriteData = new SpriteData();
            spriteData.id = i;
            spriteData.name = spriteNames[i];
            spriteData.sprite = cricketSprites[i];
            spritesData.Add(spriteData);
        }

        initialized = true;
    }



    public static Sprite GetSpriteById(int spriteId)
    {
        Initialize();
        //return the Image with the given id
        return spritesData[spriteId].sprite;

    }
    
    public static Sprite GetSpriteWithName(string spriteName)
    {
        Initialize();
        //return the Image with the given name
        return spritesData.Find(s => s.name == spriteName).sprite;
    }
    public static string GetSpriteNameById(int spriteId)
    {
        Initialize();
        return spritesData[spriteId].name;
    }

    public static int GetSpriteIdByName(string spriteName)
    {
        Initialize();
        // select the first Image with the name and return its id
        return spritesData.Find(s => s.name == spriteName).id;
    }
}
