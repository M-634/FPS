using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi.Level
{
    /// <summary>
    /// バイナリー空間分割アルゴリズムを使った、ダンジョン自動生成のエントリーポイント
    /// </summary>
    public class LevelCreator : MonoBehaviour
    {
        public int dungeonwidth;
        public int dungeonLength;

        public int roomWidthMin;
        public int roomLengthMin;

        public int maxIterations;//最大反復数
        public int corridorWidth;

        private void Start()
        {

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
