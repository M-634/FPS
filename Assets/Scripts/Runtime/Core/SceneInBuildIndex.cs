public enum SceneInBuildIndex
{
    Init = 0,//初期化シーン
    Title =1,
    Tutorial = 2,
    MainGame = 3,
    Result = 4,
    //ここから下は、BuildIndexには含まれてない
    ShowConfigure = 5,//設定画面表示
    CloseConfigure = 6,//設定画面非表示
    Exit = 7,//ゲーム終了
}
