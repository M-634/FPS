using System;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi.Level
{
    /// <summary>
    /// BSPアルゴリズムのメイン部分。空間を切るための線を引く。 空間を切る。切った空間をノードとして集める。
    /// </summary>
    public class BinarySpacePartitioner
    {
        RoomNode rootNode;
        public RoomNode RootNode => rootNode;
        public BinarySpacePartitioner(int dungeonwidth, int dungeonLength)
        {
            this.rootNode = new RoomNode(new Vector2Int(0, 0), new Vector2Int(dungeonwidth, dungeonLength), null, 0);
        }

        public List<RoomNode> PrepareNodesCollection(int maxIterations, int roomWidthMin, int roomLengthMin)
        {
            Queue<RoomNode> graph = new Queue<RoomNode>();
            List<RoomNode> listToReturn = new List<RoomNode>();
            graph.Enqueue(this.rootNode);
            listToReturn.Add(this.rootNode);
            int iterations = 0;

            while (iterations < maxIterations && graph.Count > 0)
            {
                iterations++;
                RoomNode currentNode = graph.Dequeue();
                if(currentNode.Width >= roomWidthMin * 2 || currentNode.Length >= roomLengthMin * 2)
                {
                    SplitTheSpace(currentNode, listToReturn, roomLengthMin, roomWidthMin, graph);
                }
            }

            return listToReturn;
        }

        /// <summary>
        /// 空間を切って、２つに分割する関数
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="listToReturn"></param>
        /// <param name="roomLengthMin"></param>
        /// <param name="roomWidthMin"></param>
        /// <param name="graph"></param>
        private void SplitTheSpace(RoomNode currentNode, List<RoomNode> listToReturn, int roomLengthMin, int roomWidthMin, Queue<RoomNode> graph)
        {
            Line line = GetLineDividingSpaece(currentNode.BottomLeftAreaCorner, currentNode.TopRightAreaCorner,
                roomWidthMin, roomLengthMin);

            RoomNode node1, node2;//切った後の２つの空間を用意する.
            if(line.Orientation == Orientation.Horizontal)
            {
                //横に切った場合(下を１とする)
                node1 = new RoomNode(currentNode.BottomLeftAreaCorner, new Vector2Int(currentNode.TopRightAreaCorner.x, line.Coordinates.y)
                    , currentNode
                    , currentNode.TreeLayerIndex + 1);

                node2 = new RoomNode(new Vector2Int(currentNode.BottomLeftAreaCorner.x,line.Coordinates.y),currentNode.TopRightAreaCorner
                 , currentNode
                 , currentNode.TreeLayerIndex + 1);
            }
            else
            {
                //縦に切った場合(左を１とする)
                node1 = new RoomNode(currentNode.BottomLeftAreaCorner, new Vector2Int(line.Coordinates.x, currentNode.TopRightAreaCorner.x)
                  , currentNode
                  , currentNode.TreeLayerIndex + 1); ;

                node2 = new RoomNode(new Vector2Int(line.Coordinates.x,currentNode.BottomLeftAreaCorner.y), currentNode.TopRightAreaCorner
                 , currentNode
                 , currentNode.TreeLayerIndex + 1);
            }
            AddNewNodeToCollections(listToReturn, graph, node1);
            AddNewNodeToCollections(listToReturn, graph, node2);
        }

        private void AddNewNodeToCollections(List<RoomNode> listToReturn, Queue<RoomNode> graph, RoomNode node)
        {
            listToReturn.Add(node);
            graph.Enqueue(node);
        }

        /// <summary>
        /// 空間を切る線が、縦向きか、横向きかを決めて、その線を取得する関数
        /// </summary>
        /// <param name="bottomLeftAreaCorner"></param>
        /// <param name="topRightAreaCorner"></param>
        /// <param name="roomWidthMin"></param>
        /// <param name="roomLengthMin"></param>
        /// <returns></returns>
        private Line GetLineDividingSpaece(Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, int roomWidthMin, int roomLengthMin)
        {
            Orientation orientation;
            bool lengthStatus = (topRightAreaCorner.y - bottomLeftAreaCorner.y) >= 2 * roomWidthMin;
            bool widthStatus = (topRightAreaCorner.x - bottomLeftAreaCorner.x) >= 2 * roomWidthMin;

            if(lengthStatus && widthStatus)
            {
                orientation = (Orientation)(UnityEngine.Random.Range(0, 2));
            }
            else if(widthStatus)
            {
                orientation = Orientation.Horizontal;
            }
            else
            {
                orientation = Orientation.Vertival;
            }
            return new Line(orientation,
                GetCoodinatesFororientation(orientation, bottomLeftAreaCorner, topRightAreaCorner, roomWidthMin, roomLengthMin));
        }

        /// <summary>
        /// 空間を切る線を、どこから切って行くか調整する関数
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="bottomLeftAreaCorner"></param>
        /// <param name="topRightAreaCorner"></param>
        /// <param name="roomWidthMin"></param>
        /// <param name="roomLengthMin"></param>
        /// <returns></returns>
        private Vector2Int GetCoodinatesFororientation(Orientation orientation, Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, int roomWidthMin, int roomLengthMin)
        {
            Vector2Int coordinates;
            if(orientation == Orientation.Horizontal)
            {
                coordinates = new Vector2Int(0, UnityEngine.Random.Range(bottomLeftAreaCorner.y + roomLengthMin, topRightAreaCorner.y - roomLengthMin));
            }
            else
            {
                coordinates = new Vector2Int(UnityEngine.Random.Range(bottomLeftAreaCorner.x + roomWidthMin, topRightAreaCorner.x - roomWidthMin),0);
            }
            return coordinates;
        }
    }
}