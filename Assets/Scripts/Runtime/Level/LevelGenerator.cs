using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Musashi.Level
{
    public class LevelGenerator
    {
        RoomNode rootNode;
        List<RoomNode> allNodesCollection = new List<RoomNode>();

        private int dungeonwidth;
        private int dungeonLength;

        public LevelGenerator(int dungeonwidth, int dungeonLength)
        {
            this.dungeonwidth = dungeonwidth;
            this.dungeonLength = dungeonLength;
        }

        /// <summary>
        /// 空間から定義された部屋のリストを作成する関数
        /// </summary>
        /// <param name="maxIterations"></param>
        /// <param name="roomWidthMin"></param>
        /// <param name="roomLengthMin"></param>
        /// <returns></returns>
        public List<Node> CalculateRooms(int maxIterations, int roomWidthMin, int roomLengthMin
            ,float roomBottomCornerModifier,float roomTopCornerModifier,int roomOffset)
        {
            //BSPアルゴリズムで、空間を切断していき、部屋を定義していく。
            BinarySpacePartitioner bsp = new BinarySpacePartitioner(dungeonwidth, dungeonLength);
            //木構造のデータ構造からノードを全て回収し、その名からリーフノード（部屋）を見つけてリスト化する。
            allNodesCollection = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);
            List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeafes(bsp.RootNode);

            //空間の頂点を再定義（小さくする）して、最終的な空間の頂点を決める
            RoomGenerator roomGenerator = new RoomGenerator(maxIterations, roomLengthMin, roomWidthMin);
            List<RoomNode> roomList = roomGenerator.GenerateRoomsInGivienSpaces(roomSpaces, roomBottomCornerModifier, roomTopCornerModifier, roomOffset);

            ////通路を定義してリスト化する
            //CorridorsGenrator corridorsGenrator = new CorridorsGenrator();
            //var corridorList = corridorsGenrator.CreateCorridor(allNodesCollection, corridorWidth);

            //return new List<Node>(roomList).Concat(corridorList).ToList();//合体
            return new List<Node>(roomList).ToList();
        }

        /// <summary>
        /// 空間を結ぶ通路を定義してリスト化する
        /// </summary>
        /// <param name="corridorWidth"></param>
        /// <returns></returns>
        public List<Node> CalculateCorridor(int corridorWidth)
        {
            CorridorsGenrator corridorsGenrator = new CorridorsGenrator();
            var corridorList = corridorsGenrator.CreateCorridor(allNodesCollection, corridorWidth);
            return new List<Node>(corridorList).ToList();
        }
    }
}