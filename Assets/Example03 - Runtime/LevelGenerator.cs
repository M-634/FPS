using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour {

	public NavMeshSurface surface;

	public int width = 10;
	public int height = 10;

	public GameObject wall;
	public GameObject player;

	private bool playerSpawned = false;

	// Use this for initialization
	void Start () {
		GenerateLevel();

		width = PlayerPrefs.GetInt("w", width);
		height = PlayerPrefs.GetInt("h", height);

		if (surface)
			surface.BuildNavMesh();//Update Navmesh
	}
	
	// Create a grid based level
	void GenerateLevel()
	{
		// Loop over the grid
		for (int x = 0; x <= width; x+=2)
		{
			for (int y = 0; y <= height; y+=2)
			{
				// Should we place a wall?
				if (Random.value > .7f)
				{
					// Spawn a wall
					Vector3 pos = new Vector3(x - width / 2f, 1f, y - height / 2f);
					Instantiate(wall, pos, Quaternion.identity, transform);
				} else if (!playerSpawned) // Should we spawn a player?
				{
					// Spawn the player
					Vector3 pos = new Vector3(x - width / 2f, 1.25f, y - height / 2f);
					Instantiate(player, pos, Quaternion.identity);
					playerSpawned = true;
				}
			}
		}
	}


	public void ResetLevel()
    {
		PlayerPrefs.SetInt("w", width);
		PlayerPrefs.SetInt("h", height);
		PlayerPrefs.Save();

		var curretSceneName = SceneManager.GetActiveScene().name;
		SceneManager.LoadScene(curretSceneName, LoadSceneMode.Single);
	}

}
