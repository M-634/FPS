using System;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi.Level
{
    public class LevelGenerator
    {
        RoomNode rootNode;
        List<RoomNode> allSpaceNodes = new List<RoomNode>();

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
            ,float roomBottomCornerModifier,float roomTopCornerModifier,int roomOffset,int corridorWidth)
        {
            //BSPアルゴリズムで、空間を切断していき、部屋を定義していく。
            BinarySpacePartitioner bsp = new BinarySpacePartitioner(dungeonwidth, dungeonLength);
            //木構造のデータ構造からノードを全て回収し、その名からリーフノード（部屋）を見つけてリスト化する。
            allSpaceNodes = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);
            List<Node> roomSpaces = StructurHelper.TraverseGraphToExtractLowestLeafes(bsp.RootNode);

            //部屋の大きさをランダムに再定義して、部屋のリストに加える
            //RoomGenerator roomGenerator = new RoomGenerator(maxIterations, roomLengthMin, roomWidthMin);
            //List<RoomNode> roomList = roomGenerator.GenerateRoomsInGivienSpaces(roomSpaces,roomBottomCornerModifier, roomTopCornerModifier, roomOffset);
            return new List<Node>(roomSpaces);
        }
    }
}