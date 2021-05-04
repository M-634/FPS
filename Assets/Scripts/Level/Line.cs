using UnityEngine;

namespace Musashi.Level
{
    /// <summary>
    /// 空間を切る向き
    /// </summary>
    public enum Orientation { Horizontal = 0, Vertival = 1 }
    /// <summary>
    /// 空間を切る線
    /// </summary>
    public class Line
    {
        Orientation orientation;
        Vector2Int coordinates;//x = a or y = a (aは任意の整数)の直線
        public Line(Orientation orientation,Vector2Int coordinates)
        {
            this.orientation = orientation;
            this.coordinates = coordinates;
        }

        public Orientation Orientation { get => orientation; set => orientation = value; }
        public Vector2Int Coordinates { get => coordinates; set => coordinates = value; }
    }

}