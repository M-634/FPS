using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Musashi.Level
{
    /// <summary>
    /// バイナリー空間分割アルゴリズム(BSP)を使った、ダンジョン自動生成を実行するクラス
    /// </summary>
    public class LevelCreator : MonoBehaviour
    {
        [SerializeField] NavMeshSurface surface; 

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

        [SerializeField] Transform levelParent;
        [SerializeField] GameObject wallVertical;
        [SerializeField] GameObject wallHorizontal;
        List<Vector3Int> possibleDoorVerticalPosition;
        List<Vector3Int> possibleDoorHorizontalPosition;
        List<Vector3Int> possibleWallHorizontalPosition;
        List<Vector3Int> possibleWallVerticalPosition;

        public bool doCreatWall;//test用

        private List<GameObject> levelObjectList;

        private void Start()
        {
            CreateDungeon();
        }

        public void CreateDungeon()
        {
            if (levelObjectList != null && levelObjectList.Count > 0)
                DeletDungeon();
            else
                levelObjectList = new List<GameObject>();

            if (levelParent == null) levelParent = transform;

            possibleDoorVerticalPosition = new List<Vector3Int>();
            possibleDoorHorizontalPosition = new List<Vector3Int>();
            possibleWallHorizontalPosition = new List<Vector3Int>();
            possibleWallVerticalPosition = new List<Vector3Int>();

            LevelGenerator generator = new LevelGenerator(dungeonwidth, dungeonLength);
            var listOfDungeonFloors = generator.CalculateDungeon(maxIterations, roomWidthMin, roomLengthMin,roomBottomCornerModifier,roomTopCornerModifier,roomOffset,corridorWidth);

            for(int i = 0; i < listOfDungeonFloors.Count; i++)
            {
                CreateMesh(listOfDungeonFloors[i].BottomLeftAreaCorner, listOfDungeonFloors[i].TopRightAreaCorner);
                //await Task.Delay(1000 * 1);
            }

            if(doCreatWall)
                CreateWalls(levelParent);

            if (surface)
                surface.BuildNavMesh();//bake NavMesh;
        }

        private void CreateWalls(Transform wallParent)
        {
            foreach (var wallPosotion in possibleWallHorizontalPosition)
            {
                CreateWall(wallParent, wallPosotion, wallHorizontal);
            }

            foreach (var wallPosition in possibleWallVerticalPosition)
            {
                CreateWall(wallParent, wallPosition,wallVertical);
            }
        }

        private void CreateWall(Transform wallParent, Vector3Int wallPosition,GameObject wallPrefab)
        {
            var go = Instantiate(wallPrefab, wallPosition, Quaternion.identity, wallParent);
            levelObjectList.Add(go);
        }

        private void CreateMesh(Vector2 bottomLeftCorner,Vector2 topRightCorner)
        {
            Vector3 bottomLeftVector = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
            Vector3 bottomRightVector = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
            Vector3 topLeftVector = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
            Vector3 topRightVector = new Vector3(topRightCorner.x, 0, topRightCorner.y);

            Vector3[] vertices = new Vector3[]
            {
                topLeftVector,
                topRightVector,
                bottomLeftVector,
                bottomRightVector
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
            levelFloor.transform.parent = levelParent;
            levelObjectList.Add(levelFloor);

            for (int row = (int)bottomLeftVector.x; row < (int)bottomRightVector.x; row++)
            {
                var wallPosition = new Vector3(row, 0, bottomLeftVector.z);
                AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
            }
            for (int row = (int)topLeftVector.x; row < (int)topRightCorner.x; row++)
            {
                var wallPosition = new Vector3(row, 0, topRightVector.z);
                AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
            }
            for (int col = (int)bottomLeftVector.z; col < (int)topLeftVector.z; col++)
            {
                var wallPosition = new Vector3(bottomLeftVector.x, 0, col);
                AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
            }
            for (int col = (int)bottomRightVector.z; col < (int)topRightVector.z; col++)
            {
                var wallPosition = new Vector3(bottomRightVector.x, 0, col);
                AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
            }
        }

        private void AddWallPositionToList(Vector3 wallPosition, List<Vector3Int> wallList, List<Vector3Int> doorList)
        {
            Vector3Int point = Vector3Int.CeilToInt(wallPosition);
            if (wallList.Contains(point))
            {
                doorList.Add(point);
                wallList.Remove(point);
            }
            else
            {
                wallList.Add(point);
            }
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
