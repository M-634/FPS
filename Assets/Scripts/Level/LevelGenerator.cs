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

        public List<Node> CalculateRooms(int maxIterations, int roomWidthMin, int roomLengthMin)
        {
            BinarySpacePartitioner bsp = new BinarySpacePartitioner(dungeonwidth, dungeonLength);
            allSpaceNodes = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);
            return null;
        }
    }
}