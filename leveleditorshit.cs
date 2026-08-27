using MTM101BaldAPI.AssetTools;
using PlusLevelStudio;
using PlusLevelStudio.Editor.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace NullscapeinBB
{
    static internal class leveleditorshit
    {
        static public void addEditorStuff() {
            PlusLevelStudio.EditorInterface.AddActivityVisual("null_giftcollection", NullscapeInBB.Am.Get<GameObject>("pre_Visual")).AddComponent<BoxCollider>().size = Vector3.one * 3;
            PlusLevelStudio.EditorInterface.AddObjectVisualWithCustomBoxCollider("null_giftcollectiongifthint", NullscapeInBB.Am.Get<GameObject>("pre_Visual"), Vector3.one * 3, Vector3.zero);
            
            EditorInterfaceModes.AddModeCallback((mode, idk) =>
            {
                EditorInterfaceModes.AddToolToCategory(mode, "activities", new ActivityTool("null_giftcollection", AssetLoader.SpriteFromMod(NullscapeInBB.Instance, Vector2.one / 2, 35, "items", "placeholder.png"), 0f));
                EditorInterfaceModes.AddToolToCategory(mode, "activities", new ObjectToolNoRotation("null_giftcollectiongifthint", AssetLoader.SpriteFromMod(NullscapeInBB.Instance, Vector2.one / 2, 35, "items", "placeholder.png"),0f));
            });
        }
    }
}
