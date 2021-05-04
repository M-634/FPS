using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi.Level
{
    /// <summary>
    /// バイナリー空間分割アルゴリズムを使った、ダンジョン自動生成を実行するクラス
    /// </summary>
    public class LevelCreator : MonoBehaviour
    {
        public int dungeonwidth;
        public int dungeonLength;

        public int roomWidthMin;
        public int roomLengthMin;

        public int maxIterations;//空間を何回切断するか。木構造の深さ（level）に相当する
        public int corridorWidth;

        private void Start()
        {
            CreateDungeon();
        }

        private void CreateDungeon()
        {
            LevelGenerator generator = new LevelGenerator(dungeonwidth, dungeonLength);
            var listOfRooms = generator.CalculateRooms(maxIterations, roomWidthMin, roomLengthMin);
        }

        private void Update()
        {

        }
    }
}
