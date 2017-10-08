using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;

   //Room class for generating the sprite/Texture map.
    //Also contains the game object prefabs as a list, populate it in the ui.
    [System.Serializable]
    public class Room
    {
        public Room(Vector3 pos, Color32 color)
        {
            this.pos = pos;
            this.color = color;
        }
        public Vector3 pos;
        public Color32 color;
    public int doorCount = 0;
    }

    [System.Serializable]
    public class RoomPrefab
    {
        public Color32 color;
        public string type;
        public GameObject[] floorPrefabs;
    }

class Utils
{
    public enum enemyName{
        Warrior,
        Hammer,
        Archer
    }

    public enum clipType
    {
        awake,
        attack,
        die   
    }
    public static Vector3 RoomPosToWorldPosition(Vector3 position) {
        return new Vector3(position.x * 17, position.y, position.z * 13);
    }

    public static void playEnemyAudioClip(enemyName name, clipType type) {
        GameObject musicBox = GameObject.FindGameObjectWithTag("MusicBox");
        AudioSource audioSource = musicBox.GetComponent<AudioSource>();

        if (type == clipType.awake) {
            audioSource.clip = musicBox.GetComponent<AudioControls>().Clips[0];
        } else if (type == clipType.die) {
            audioSource.clip = musicBox.GetComponent<AudioControls>().Clips[1];
        } else if (type == clipType.attack) {
           /* TODO IF YOU GET TIME ADD THESE BACK, needs work if (name == enemyName.Warrior)
                audioSource.clip = musicBox.GetComponent<AudioControls>().Clips[2]; //Heavy Swing?
            else if (name == enemyName.Hammer)
                audioSource.clip = musicBox.GetComponent<AudioControls>().Clips[3]; //Heavy Swing?
            else if (name == enemyName.Archer)
                audioSource.clip = musicBox.GetComponent<AudioControls>().Clips[4]; //Draw Bow*/
        }

        audioSource.Play();

    }
}

