using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Callbacks;//OnOpenAssetを使うのに必要
using UnityEditor;
using System.IO;
using System.Diagnostics;


/// <summary>
/// 特定のアプリケーションを、Unityから開けるようにするクラス
/// </summary>
public class SpecificOpenApp : Editor
{
    /// <summary>
    /// .shader、.hlslファイルをvscodeで開く(自分用)
    /// </summary>
    public static class OpneVSCode
    {
        private const string SHADERFile = ".shader";
        private const string HLSLFile = ".hlsl";

        [OnOpenAsset(0)]
        public static bool OnOpen(int instanceID, int line)
        {
            //開いたアセットを取得
            string path = AssetDatabase.GetAssetPath(instanceID);

            //拡張子が.shaderまたは、hlslでないならそのままファイルを開く
            if (Path.GetExtension(path) != SHADERFile && Path.GetExtension(path) != HLSLFile) return false;

            string fullPath = Path.GetFullPath(path);

            Process.Start("code", fullPath);
            return true;
        }
    }

    public class OpenPhotoshop : Editor
    {
        const string ItemName = "Assets/Open in Photoshop";

        [MenuItem(ItemName, false)]
        static void Opne()
        {
            foreach (var guid in Selection.assetGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var fullPath = Path.GetFullPath(path);
                Process.Start("photoshop", fullPath);
            }
        }
    }
}