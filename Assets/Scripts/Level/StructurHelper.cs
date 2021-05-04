using System;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi.Level
{
    public static class StructurHelper
    {
        /// <summary>
        /// 木構造の子どもを持たないノード（leaf）部分を集めて、リスト化する関数。
        /// (ゲーム上では、このノード部分が部屋となる)
        /// </summary>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        public static List<Node> TraverseGraphToExtractLowestLeafes(RoomNode parentNode)
        {
            Queue<Node> nodesToCheck = new Queue<Node>();//子どもノードを持っているかチェックするキュー
            List<Node> listToReturn = new List<Node>();//部屋となるノードのリスト

            if (parentNode.ChildrenNodeList.Count == 0)
            {
                return new List<Node>() { parentNode };
            }

            foreach (var child in parentNode.ChildrenNodeList)
            {
                nodesToCheck.Enqueue(child);
            }

            //find leaf nodes
            while (nodesToCheck.Count > 0)
            {
                var currentNode = nodesToCheck.Dequeue();
                if (currentNode.ChildrenNodeList.Count == 0)
                {
                    listToReturn.Add(currentNode);
                }
                else
                {
                    //子どものノードをチェックリストに追加する
                    foreach (var child in currentNode.ChildrenNodeList)
                    {
                        nodesToCheck.Enqueue(child);
                    }
                }
            }
            return listToReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Vector2Int GeneraterBottomLeftConerBetween(Vector2Int boundaryLeftPoint, Vector2Int boundaryRightPoint, float pointModifier, int offset)
        {
            int minX = boundaryLeftPoint.x + offset;
            int maxX = boundaryRightPoint.x - offset;
            int minY = boundaryLeftPoint.y + offset;
            int maxY = boundaryRightPoint.y - offset;

            return new Vector2Int(
                UnityEngine.Random.Range(minX, (int)(minX + (maxX - minX) * pointModifier)),
                UnityEngine.Random.Range(minY, (int)(minY + (maxY - minY) * pointModifier))
                );
        }

        /// <summary>
        /// 
        /// </summary>
        public static Vector2Int GeneraterTopRightCornerConerBetween(Vector2Int boundaryLeftPoint, Vector2Int boundaryRightPoint, float pointModifier, int offset)
        {
            int minX = boundaryLeftPoint.x + offset;
            int maxX = boundaryRightPoint.x - offset;
            int minY = boundaryLeftPoint.y + offset;
            int maxY = boundaryRightPoint.y - offset;

            return new Vector2Int(
                UnityEngine.Random.Range((int)(minX + (maxX - minX) * pointModifier),maxX),
                UnityEngine.Random.Range((int)(minY + (maxY - minY) * pointModifier),maxY)
                );
        }


        public static Vector2Int CalculateMiddlePoint(Vector2Int v1, Vector2Int v2)
        {
            Vector2 sum = v1 + v2;
            Vector2 tempVector = sum / 2;
            return new Vector2Int((int)tempVector.x, (int)tempVector.y);
        }

        public static int CalculateMiddlePoint(int v1,int v2)
        {
            return (v1 + v2) / 2;//余りは切り捨て
        }
    }

    public enum RelativePosition
    {
        Up,
        Down,
        Right,
        Left
    }
}