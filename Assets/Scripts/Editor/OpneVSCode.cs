using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Callbacks;//OnOpenAssetを使うのに必要
using UnityEditor;
using System.IO;
using System.Diagnostics;

namespace Musashi
{
    /// <summary>
    /// .shader拡張子ファイルをダブルクリックするとVSCodeを開くようにする
    /// </summary>
    public static class OpneVSCode
    {
        private const string EXTENSION = ".shader";
        private const string VSCODE_ABSOLUTEPATH = @"C:\Users\miyan\AppData\Local\Programs\Microsoft VS Code\Code.exe";

        [OnOpenAsset(0)]
        public static bool OnOpen(int instanceID,int line)
        {
            //開いたアセットを取得
            string path = AssetDatabase.GetAssetPath(instanceID);

            //拡張子が.shaderでないならそのままファイルを開く
            if (Path.GetExtension(path) != EXTENSION) return false;

            string fullPath = Path.GetFullPath(path);
        
            Process.Start(VSCODE_ABSOLUTEPATH, fullPath);
            return true;
        }
        
    }
}