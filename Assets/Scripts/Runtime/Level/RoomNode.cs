using UnityEngine;

namespace Musashi.Level
{
    /// <summary>
    /// 部屋（空間）を示すクラス
    /// </summary>
    public class RoomNode : Node
    {
        public RoomNode(Vector2Int bottomLeftAreaCorner,Vector2Int topRightAreaCorner,Node parentNode,int index) : base(parentNode)
        {
            this.BottomLeftAreaCorner = bottomLeftAreaCorner;
            this.TopRightAreaCorner = topRightAreaCorner;
            this.BottomRightAreaCorner = new Vector2Int(topRightAreaCorner.x, bottomLeftAreaCorner.y);
            this.TopLeftAreaCorner = new Vector2Int(bottomLeftAreaCorner.x, TopRightAreaCorner.y);
            this.TreeLayerIndex = index;
        }

        public int Width => TopRightAreaCorner.x - BottomLeftAreaCorner.x;
        public int Length => TopRightAreaCorner.y - BottomLeftAreaCorner.y;
    }
}