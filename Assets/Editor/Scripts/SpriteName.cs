using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public class SpriteName : MonoBehaviour {
    [MenuItem("Custom/Update Sprite Name")]
    static void UpdateName() {
        foreach (var obj in Selection.objects) {
            if (obj is Texture2D) {
                var factory = new SpriteDataProviderFactories();
                factory.Init();
                var dataProvider = factory.GetSpriteEditorDataProviderFromObject(obj);
                dataProvider.InitSpriteEditorDataProvider();

                SetSpriteName(dataProvider, obj.name);

                dataProvider.Apply();
               
                var assetImporter = dataProvider.targetObject as AssetImporter;
                assetImporter.SaveAndReimport();
            }
        }
    }

    static void SetSpriteName(ISpriteEditorDataProvider dataProvider, string textureName) {
        var spriteRects = dataProvider.GetSpriteRects();
        for (var i = 0; i < spriteRects.Length; ++i) {
            spriteRects[i].name = $"{textureName}_{i}";
        }
        dataProvider.SetSpriteRects(spriteRects);
       
        // Additional step for Unity 2021.2 and newer
        var nameFileIdDataProvider = dataProvider.GetDataProvider<ISpriteNameFileIdDataProvider>();
        var pairs = nameFileIdDataProvider.GetNameFileIdPairs();
        foreach (var pair in pairs) {
            var spriteRect = System.Array.Find(spriteRects, x => x.spriteID == pair.GetFileGUID());
            pair.name = spriteRect.name;
        }
       
        nameFileIdDataProvider.SetNameFileIdPairs(pairs);
        // End
    }

    static string GetRandomName() {
        return GUID.Generate().ToString();
    }
}
