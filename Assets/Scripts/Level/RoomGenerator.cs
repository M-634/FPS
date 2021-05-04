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

        /// <summary>
        /// 空間の頂点を再定義（小さくなるように）する関数
        /// memo：もう少し修正が必要です。
        /// </summary>
        /// <param name="roomSpaces"></param>
        /// <param name="roomBottomCornerModifier"></param>
        /// <param name="roomTopCornerModiifer"></param>
        /// <param name="roomOffset"></param>
        /// <returns></returns>
        public List<RoomNode> GenerateRoomsInGivienSpaces(List<Node> roomSpaces,float roomBottomCornerModifier,float roomTopCornerModiifer,int roomOffset)
        {
            List<RoomNode> listToReturn = new List<RoomNode>();
            foreach (var space in roomSpaces)
            {
                //部屋の各頂点を、部屋が元の大きさより小さくなるように再定義する。
                //Vector2Int newBottomLeftPoint = StructurHelper.GeneraterBottomLeftConerBetween(space.BottomLeftAreaCorner, 
                //    space.TopRightAreaCorner, roomBottomCornerModifier, roomOffset);
                //Vector2Int newTopRightPont = StructurHelper.GeneraterTopRightCornerConerBetween(space.BottomLeftAreaCorner,
                //    space.TopRightAreaCorner, roomTopCornerModiifer, roomOffset) 

                //頂点を再設定
                //space.BottomLeftAreaCorner = newBottomLeftPoint;
                //space.TopRightAreaCorner = newTopRightPont;
                //space.BottomRightAreaCorner = new Vector2Int(newTopRightPont.x, newBottomLeftPoint.y);
                //space.TopLeftAreaCroner = new Vector2Int(newBottomLeftPoint.x, newTopRightPont.y);
                //listToReturn.Add((RoomNode)space);

                //部屋の各頂点を、部屋が元の大きさより小さくなるように再定義する。
                space.BottomLeftAreaCorner = new Vector2Int(space.BottomLeftAreaCorner.x + roomOffset, space.BottomLeftAreaCorner.y + roomOffset);
                space.TopRightAreaCorner = new Vector2Int(space.TopRightAreaCorner.x - roomOffset, space.TopRightAreaCorner.y - roomOffset);
                space.BottomRightAreaCorner = new Vector2Int(space.TopRightAreaCorner.x - roomOffset, space.BottomLeftAreaCorner.y + roomOffset);
                space.TopLeftAreaCroner = new Vector2Int(space.BottomLeftAreaCorner.x + roomOffset, space.TopRightAreaCorner.y - roomOffset);

                listToReturn.Add((RoomNode)space); 
            }
            return listToReturn;
        }
    }
}