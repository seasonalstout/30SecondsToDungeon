using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;




public class LevelBuilder : MonoBehaviour
{

    /** At a minimum this will contian 10 normal rooms, 1 start room,
     * 1 boss room and 4 key rooms.
     **/
    //1 of these, write the controls on the floor. 
    public RoomPrefab[] roomPrefabs;

    public GameObject horizontalWall;
    public GameObject verticalWall;

    public GameObject horizontalDoor;
    public GameObject horizontalBossDoor;
    public GameObject verticalBossDoor;
    public GameObject verticalDoor;

    List<string> placedPrefabs = new List<string>();
    //Creates the grid we will generate a dungeon in, they populates possiblePositions
    private int maxWidth = 8;
    private int maxHeight = 8;
    private int maxRooms = 10;
    private int maxErrors = 25;
    List<Room> rooms = new List<Room>();
    //Leave the loop in for debugging purposes. If >1 do not generate a dungeon, instead spit out a png
    int numberOfDungeons = 1;
    int spawnHitCount = 0;

    private int bufferPosX = 0;
    private int bufferPosY = 0;

    private bool bossDoorSpawned = false;
    void Start()
    {
        Texture2D levelMapLoop = new Texture2D(maxWidth * numberOfDungeons, maxHeight * numberOfDungeons);
        for (int i = 0; i < numberOfDungeons; i++) {
            GenerateMap(i, levelMapLoop);
        }
    }

    void EmptyMap() {
        //Find all children and destroy them
        while (transform.childCount > 0) {
            Transform child = transform.GetChild(0);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
    }

    void GenerateMap(int mapNum, Texture2D levelMap)
    {
        EmptyMap();

        int keyRoom = Random.Range(5, maxRooms - 1);
        int lootRoom = Random.Range(1, 4);

        //Take a random point in the map as our start room. Start room is green
        Room currentRoom = new Room(new Vector2(Mathf.Abs(maxWidth/2), 0), Color.black);
       // rooms.Add(currentRoom);
        
        //Left is 0, right is 1, up is 2, down is 3.
        for (int numRooms = 0; numRooms < maxRooms; numRooms++)
        {
            int currentErrors = 0;
            //Loop until a position is valid. (exists in possilbe rooms, is not outside range)
            bool isValid = false;
            bool hasCreatedADoor = false;

            while (isValid != true) {
                //Pick a room direction
                int nextPos = Random.Range(0, 3);

                if (numRooms < 2 || numRooms == maxRooms -1) //always start with up, end with up
                    nextPos = 2;
                Room newRoom = new Room(currentRoom.pos, currentRoom.color);

                if (nextPos == 0) {
                    if (newRoom.pos.x - 1 >= 0)
                        newRoom.pos.x--;
                } else if (nextPos == 1) {
                    if (newRoom.pos.x + 1 <= maxWidth)
                        newRoom.pos.x++;
                } else if (nextPos == 2) {
                    if (newRoom.pos.z + 1 <= maxHeight)
                        newRoom.pos.z++;
                } else if (nextPos == 3) {
                    if (newRoom.pos.z - 1 >= 0)
                        newRoom.pos.z--;
                }

                bool bFound = false;
                for (int i = 0; i < rooms.Count; i++)
                {
                    if (rooms[i].pos == newRoom.pos) //Check to see if there is already a room there.
                    {
                        bFound = true;

                    }
                }
                if (!bFound)
                { //There isnt so add one, set it to current, step out
                    rooms.Add(newRoom);
                    currentRoom = newRoom;
                    hasCreatedADoor = true;

                    isValid = true;
                } else
                {

                    if (!hasCreatedADoor)
                    {
                        // SpawnDoor(currentRoom, nextPos);
                    }
                    currentErrors++;
                    //We tried enough, time to give up and select another tile randomly, this could get wonky.
                    if (currentErrors >= maxErrors) {
                        currentErrors = 0;
                        currentRoom = rooms[Random.Range(0, rooms.Count)];
                    }
                }
            }
        }
        //Paint the rooms, set color on each significant room.
        rooms[0].color = Color.green;
        rooms[keyRoom].color = new Color(255,255,0,255);
        rooms[lootRoom].color = Color.blue;
        rooms[rooms.Count - 1].color = Color.red;
        //GetComponent<CameraControls>().MoveToRoom(rooms[0]);

        //This is the debugging code. Spits out a texture map of rooms. If we arent debugging we've already created the next room.
        if (numberOfDungeons > 1)
        {
            foreach (Room room in rooms)
            {
                levelMap.SetPixel((int)room.pos.x + bufferPosX, (int)room.pos.y + +bufferPosY, room.color);
            }
            if (bufferPosX > 50)
            {
                bufferPosY += 10;
                bufferPosX = 0;
            }
            else
            {
                bufferPosX += 10;
            }
            rooms.Clear();
            File.WriteAllBytes("D:\\depot\\StubbornMule\\unity\\BattleCards\\BattleCards\\generated\\temp.png", levelMap.EncodeToPNG());
        } else {
            foreach (Room room in rooms)
            {
                SpawnRoom(room);
            }
        }
        //Not sure what this will look like. Start by placing generic wall on all intersections, work out the logic later
    }


    //Functrion to randomize which empty room tile is placed. There is no weighting in pass 1
    // void RandomizeEmptyRooms(GameObject floor, int grassX, int grassY)
    //{
    //    int i = Random.Range(0, emptyRooms.Length);
    //   floor = emptyRooms[i];

    /*use to random grass on top, could be useful for detailing
    i = Random.Range(0, 2);
    if (i == 1)
    {
        i = Random.Range(0, grassSprites.Length);
        GameObject grassObj = (GameObject)Instantiate(grassPrefab, new Vector3(grassX, grassY, 0), Quaternion.identity);
        grassObj.GetComponent<SpriteRenderer>().sprite = grassSprites[i];
    }*/
    // }

    public List<Room>GetRooms()
    {
        return rooms;
    }

    void SpawnRoom(Room spawnRoom)
    {
        //Find color in map
        foreach (RoomPrefab roomPrefab in roomPrefabs)
        {
            if (roomPrefab.color.Equals(spawnRoom.color))
            {
                GameObject floorTile = roomPrefab.floorPrefabs[Random.Range(0, roomPrefab.floorPrefabs.Length)];
                if (placedPrefabs.Contains(floorTile.name))//Not a hard and fast rule but if the tile exists try again, we dont care about dups after the first
                    floorTile = roomPrefab.floorPrefabs[Random.Range(0, roomPrefab.floorPrefabs.Length)];

                placedPrefabs.Add(floorTile.name);


                GameObject obj = (GameObject)Instantiate(floorTile, Utils.RoomPosToWorldPosition(spawnRoom.pos), Quaternion.identity);
                for (int i = 0; i <=3; i++)
                {
                    SpawnWallOrDoor(spawnRoom, i);
                }
                return;
            }
        }
        Debug.LogError("Could not find prefab for color: " + spawnRoom.color.ToString());
    }

    /**
     * 0 = left
     * 1 = right
     * 2= up
     * 3 = down
     **/
    //Right now I'm just spawning doors on all intersects, otherwise walls.
    void SpawnWallOrDoor(Room room, int direction)
    {
        Vector3 position = Utils.RoomPosToWorldPosition(room.pos);
        position.y += 1.5f; //Is a floor position, elevate to wall height
        Vector3 roomCheck = room.pos;
        bool isHorizontal = false;

        if (direction == 0)
        {
            roomCheck.x -= 1;
            position.x -= 8.5f;
        }
        else if (direction == 1)
        {
            roomCheck.x += 1;
            position.x += 8.5f;
        }
        else if (direction == 2)
        {
            roomCheck.z += 1;
            position.z += 6.5f;
            isHorizontal = true;

        }
        else if (direction == 3)
        {
            roomCheck.z -= 1;
            position.z -= 6.5f;
            isHorizontal = true;
        }
        //Check if there is an adjacent room before spawn

        if (IsARoomHere(roomCheck) && room.doorCount < 3) {
            if (room.color == Color.red) {
                SpawnBossDoor(position, isHorizontal);
            } else {
                if (IsSpawnRoomHere(roomCheck) )
                {
                    spawnHitCount++;
                    if (spawnHitCount > 1) // if were passed room 1, all spawn intersections have a wall
                        SpawnWall(position, isHorizontal, true);
                }
                else
                {
                    SpawnDoor(position, isHorizontal);
                    room.doorCount++;
                }
            }
        }
        else
        {
            if (room.color != Color.green ) //Dont spawn walls around the spawn room
                SpawnWall(position, isHorizontal);
        }
    }

    void SpawnDoor(Vector3 pos, bool isHorizontal)
    {
        GameObject door = horizontalDoor;
        if (!isHorizontal)
            door = verticalDoor;

        if (!IsWallOrDoorHere(pos))
        {
           GameObject obj = (GameObject)Instantiate(door, pos, Quaternion.identity);
        }
    }

    void SpawnBossDoor(Vector3 pos, bool isHorizontal)
    {
        GameObject door = horizontalBossDoor;
        if (bossDoorSpawned)
            door = horizontalWall;
        if (!isHorizontal)
        {
            door = verticalBossDoor;
            if (bossDoorSpawned)
                verticalBossDoor = verticalWall;

        }

        //Ony overwrite doors
        if (IsDoorHere(pos))
        {
            //This is the last room constructed so doors will have been placed/place fillers in.
            GameObject obj = (GameObject)Instantiate(door, pos, Quaternion.identity);
            bossDoorSpawned = true;
        }
        
    }


    void SpawnWall(Vector3 pos, bool isHorizontal, bool force = false)
    {
        GameObject wall = horizontalWall;
        if (!isHorizontal)
            wall = verticalWall;

        if (!IsWallOrDoorHere(pos) || force)
        {
            GameObject obj = (GameObject)Instantiate(wall, pos, Quaternion.identity);
        }
    }

    bool IsARoomHere(Vector3 checkRoom)
    {
        int numHits = 0;
          foreach (Room room in rooms) {
            if (room.pos.Equals(checkRoom))
                numHits ++;
        }
        if (numHits < 1)
            return false;
        else
            return true;
    }

    bool IsSpawnRoomHere(Vector3 checkRoom)
    {
        int numHits = 0;
        foreach (Room room in rooms)
        {
            if (room.pos.Equals(checkRoom) && room.color == Color.green)
                numHits++;
        }
        if (numHits < 1)
            return false;
        else
            return true;
    }

    bool IsWallOrDoorHere(Vector3 checkPos)
    {
        Collider[] colliders;
        if ((colliders = Physics.OverlapSphere(checkPos, 0.1f)).Length > 1) 
            //Presuming the object you are testing also has a collider 0 otherwise
        {
            foreach (var collider in colliders)
            {
                var go = collider.gameObject; //This is the game object you collided with
                if (go == gameObject) continue; //Skip the object itself
                if (go.CompareTag("InvisWall"))
                {
                    Debug.Log("found invis");
                 
                };
                return true;
            }
        }
        return false;
    }

    bool IsDoorHere(Vector3 checkPos)
    {
        Collider[] colliders;
        if ((colliders = Physics.OverlapSphere(checkPos, 0.5f)).Length > 1)
        //Presuming the object you are testing also has a collider 0 otherwise
        {
            foreach (var collider in colliders)
            {
                var go = collider.gameObject; //This is the game object you collided with
                if (go == gameObject || go.CompareTag("Wall")) continue; //Skip the object itself and walls

                return true;
            }
        }
        return false;
    }


    //Find all room intersections and create doors
    void LoadDoors()
    {
    }
    /* Could be useful for READY!!? GO!!!!! start
    public void ToggleLevelMenu()
    {
        menuShowing = !menuShowing;
        foreach (GameObject button in Buttons)
        {
            button.SetActive(menuShowing);
        }
    }*/

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SkipLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

