using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Musashi.Level
{
    /// <summary>
    /// バイナリー空間分割アルゴリズム(BSP)を使った、ダンジョン自動生成を実行するクラス
    /// </summary>
    public class LevelCreator : MonoBehaviour
    {
        /// <summary>最初に定義する空間の幅</summary>
        [SerializeField] int dungeonwidth;
        /// <summary>最初に定義する空間の奥行</summary>
        [SerializeField] int dungeonLength;
        /// <summary>生成される部屋の最小の幅</summary>
        [SerializeField] int roomWidthMin;
        /// <summary>生成される部屋の最小の奥行</summary>
        [SerializeField] int roomLengthMin;

        /// <summary> 空間を何回切断するか。木構造の深さ（level）に相当する</summary>
        [SerializeField] int maxIterations;
        /// <summary>生成される廊下の幅</summary>
        [SerializeField] int corridorWidth;
        /// <summary>ステージのマテリアル</summary>
        [SerializeField] Material material;
        [Range(0.0f, 0.3f)]
        [SerializeField] float roomBottomCornerModifier;
        [Range(0.7f, 1.0f)]
        [SerializeField] float roomTopCornerModifier;
        [Range(0, 2)]
        [SerializeField] int roomOffset;
        [SerializeField] GameObject wallVertical; 
        [SerializeField] GameObject wallHorizontal;
        List<Vector3Int> possibleDoorVerticalPosition;
        List<Vector3Int> possibleDoorHorizontalPosition;
        List<Vector3Int> possibleWallHorizontalPosition;
        List<Vector3Int> possibleWallVerticalPosition;




        private List<GameObject> levelObjectList;

        private void Start()
        {
            CreateDungeon();
        }

        public async void CreateDungeon()
        {
            if (levelObjectList != null && levelObjectList.Count > 0)
                DeletDungeon();
            else
                levelObjectList = new List<GameObject>();

            LevelGenerator generator = new LevelGenerator(dungeonwidth, dungeonLength);
            var listOfRooms = generator.CalculateRooms(maxIterations, roomWidthMin, roomLengthMin,roomBottomCornerModifier,roomTopCornerModifier,roomOffset,corridorWidth);

            for(int i = 0; i < listOfRooms.Count; i++)
            {
                CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
                await Task.Delay(1000 * 1);
            }
        }

        private void CreateMesh(Vector2 bottomLeftCorner,Vector2 topRightCorner)
        {
            Vector3 bottomLeftVector = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
            Vector3 bottomRightVecor = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
            Vector3 topLeftVector = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
            Vector3 topRightVector = new Vector3(topRightCorner.x, 0, topRightCorner.y);

            Vector3[] vertices = new Vector3[]
            {
                topLeftVector,
                topRightVector,
                bottomLeftVector,
                bottomRightVecor
            };

            Vector2[] uvs = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
            }

            
            int[] triangles = new int[]
            {
                //三角形の頂点番号
                0,1,2,
                2,1,3
            };

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            GameObject levelFloor = new GameObject("Mesh" + bottomLeftCorner + topRightCorner, typeof(MeshFilter), typeof(MeshRenderer));

            levelFloor.transform.position = Vector3.zero;
            levelFloor.transform.localScale = Vector3.one;
            levelFloor.GetComponent<MeshFilter>().mesh = mesh;
            levelFloor.GetComponent<MeshRenderer>().material = material;
            levelFloor.transform.parent = transform;
            levelObjectList.Add(levelFloor);
        }

        
        public void DeletDungeon()
        {
            if(Application.isPlaying)
                levelObjectList.ForEach(obj => Destroy(obj));
            else
                levelObjectList.ForEach(obj => DestroyImmediate(obj));
        }

    }
}
