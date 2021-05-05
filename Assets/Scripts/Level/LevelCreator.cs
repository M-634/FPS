using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

namespace Musashi.Level
{
    /// <summary>
    /// ダンジョン自動生成を実行するクラス
    /// </summary>
    public class LevelCreator : MonoBehaviour
    {
        [SerializeField] NavMeshSurface surface;
        [SerializeField] GameObject player;
        [SerializeField, Range(0, 2)] float playerSpwanOffset = 2f;
        [SerializeField] GameObject enemySpwaner;
        [SerializeField, Range(0, 2)] float enemySpwanerSpwanOffset = 1.5f;

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
        [SerializeField] Material levelFloorMaterial;

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

        public async void CreateDungeon()
        {
            DeletDungeon();

            if (levelParent == null) levelParent = transform;

            possibleDoorVerticalPosition = new List<Vector3Int>();
            possibleDoorHorizontalPosition = new List<Vector3Int>();
            possibleWallHorizontalPosition = new List<Vector3Int>();
            possibleWallVerticalPosition = new List<Vector3Int>();

            LevelGenerator generator = new LevelGenerator(dungeonwidth, dungeonLength);
            var listOfRoomFloors = generator.CalculateRooms(maxIterations, roomWidthMin, roomLengthMin, roomBottomCornerModifier, roomTopCornerModifier, roomOffset);
            var listOfCorridors = generator.CalculateCorridor(corridorWidth);
            var listOfDungeonFloors = new List<Node>(listOfRoomFloors).Concat(listOfCorridors).ToList();

            for (int i = 0; i < listOfDungeonFloors.Count; i++)
            {
                CreateMesh(listOfDungeonFloors[i].BottomLeftAreaCorner, listOfDungeonFloors[i].TopRightAreaCorner);
                await Task.Delay(1);
            }
            CombineFloorMesh();

            if (doCreatWall)
                CreateWalls(levelParent);

            //bake NavMesh;
            if (surface)
                surface.BuildNavMesh();


            //Instanciate enemy spwaner in room of midpoint
            if (enemySpwaner)
            {
                var manager = GameManager.Instance;
                if(manager)
                    manager.SumOfEnemySpwaner = 0;
                for (int i = 1; i < listOfRoomFloors.Count; i++)
                {
                    SpwanObjectInRoom(enemySpwaner, listOfRoomFloors[i], enemySpwanerSpwanOffset);
                    if (manager)
                        manager.SumOfEnemySpwaner++;
                }
            }

            //Spwan player
            if (player)
            {
                SpwanObjectInRoom(player, listOfRoomFloors[0], playerSpwanOffset);
            }
        }

        private void SpwanObjectInRoom(GameObject spwanObject, Node node, float heightOffset)
        {
            var midPoint = StructureHelper.CalculateMiddlePoint(node.BottomLeftAreaCorner, node.TopRightAreaCorner);
            var go = Instantiate(spwanObject, new Vector3(midPoint.x, heightOffset, midPoint.y), Quaternion.identity);
            levelObjectList.Add(go);
        }

        private void CombineFloorMesh()
        {
            MeshFilter[] meshFilters = levelParent.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];

            for (int i = 0; i < meshFilters.Length; i++)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                DestroyImmediate(meshFilters[i].gameObject);
            }

            GameObject newObj = new GameObject("CombineMeshFloor", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
            var mesh = newObj.GetComponent<MeshFilter>().sharedMesh = new Mesh();
            newObj.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
            newObj.GetComponent<MeshCollider>().sharedMesh = mesh;
            newObj.GetComponent<MeshRenderer>().material = levelFloorMaterial;
            newObj.transform.parent = levelParent;

            levelObjectList.Clear();
            levelObjectList.Add(newObj);
        }

        private void CreateWalls(Transform wallParent)
        {
            foreach (var wallPosotion in possibleWallHorizontalPosition)
            {
                CreateWall(wallParent, wallPosotion, wallHorizontal);
            }

            foreach (var wallPosition in possibleWallVerticalPosition)
            {
                CreateWall(wallParent, wallPosition, wallVertical);
            }
        }

        private void CreateWall(Transform wallParent, Vector3Int wallPosition, GameObject wallPrefab)
        {
            var go = Instantiate(wallPrefab, wallPosition, Quaternion.identity, wallParent);
            levelObjectList.Add(go);
        }

        private void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
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

            GameObject levelFloor = new GameObject("Floor" + bottomLeftCorner + topRightCorner, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));

            levelFloor.transform.position = Vector3.zero;
            levelFloor.transform.localScale = Vector3.one;
            levelFloor.GetComponent<MeshFilter>().mesh = mesh;
            levelFloor.GetComponent<MeshRenderer>().material = levelFloorMaterial;
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
            if (levelObjectList == null)
            {
                levelObjectList = new List<GameObject>();
                return;
            }

           if(levelObjectList.Count > 0)
                levelObjectList.ForEach(obj => DestroyImmediate(obj));
        }

    }
}
