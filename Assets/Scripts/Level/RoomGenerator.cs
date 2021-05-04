using System;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi.Level
{
    public class RoomGenerator
    {
        private int maxIterations;
        private int roomLengthMin;
        private int roomWidthMin;

        public RoomGenerator(int maxIterations, int roomLengthMin, int roomWidthMin)
        {
            this.maxIterations = maxIterations;
            this.roomLengthMin = roomLengthMin;
            this.roomWidthMin = roomWidthMin;
        }

        public List<RoomNode> GenerateRoomsInGivienSpaces(List<Node> roomSpaces)
        {
            List<RoomNode> listToReturn = new List<RoomNode>();
            foreach (var space in roomSpaces)
            {
                //部屋の各頂点を、部屋が元の大きさより小さくなるように再定義する。
                Vector2Int newBottomLeftPoint = StructurHelper.GeneraterBottomLeftConerBetween(space.BottomLeftAreaCorner, 
                    space.TopRightAreaCorner, 0.1f, 1);
                Vector2Int newTopRightPont = StructurHelper.GeneraterTopRightCornerConerBetween(space.BottomLeftAreaCorner,
                    space.TopRightAreaCorner, 0.9f, 1);

                //頂点を再設定
                space.BottomLeftAreaCorner = newBottomLeftPoint;
                space.TopRightAreaCorner = newTopRightPont;
                space.BottomRightAreaCorner = new Vector2Int(newTopRightPont.x, newBottomLeftPoint.y);
                space.TopLeftAreaCroner = new Vector2Int(newBottomLeftPoint.x, newTopRightPont.y);
                listToReturn.Add((RoomNode)space);
            }
            return listToReturn;
        }
    }
}