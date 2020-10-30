using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SaveSate save = new SaveSate();

        for (int i = 0; i < 3; i++)
        {
            int magSize = Random.Range(50, 100);
            Weapon w = new Weapon("Weapon " + i, Random.Range(20, 200), magSize, magSize, Random.Range(10, 100));
            save.AllWeapons.Add(w);
        }
        Enemy e = new Enemy("Enemy 1", new Vector3(Random.Range(0, 200), Random.Range(0, 200), Random.Range(0, 200)), new Quaternion(0, Random.value, 0, 0), Random.Range(10, 500), "Attacking", 0.6f);
        Enemy e2 = new Enemy("Enemy 2", new Vector3(Random.Range(0, 200), Random.Range(0, 200), Random.Range(0, 200)), new Quaternion(0, Random.value, 0, 0), Random.Range(10, 500), "Walking", 0.6f);
        save.AllEnemies.Add(e);
        save.AllEnemies.Add(e2);

        string[] playerInventory = { "Weapon", "First-Aid Kit", "Grenade"};
        Player player = new Player(56, 0, 12, playerInventory);
        save.PlayerState = player;

        List<Door> doorStates = new List<Door>();
        for (int i = 0; i < 3; i++)
        {
            Door door = new Door(true, Random.Range(0, 15));
            doorStates.Add(door);
        }
        LevelState levelState = new LevelState("Level 1", doorStates);
        save.LevelState = levelState;

        string jsonSave = JsonUtility.ToJson(save);

        Debug.Log(jsonSave);
    }
    [System.Serializable]
    public class LevelState
    {
        public string currentMap;
        public List<Door> doorStates;

        public LevelState()
        {
        }

        public LevelState(string _currentMap, List<Door> _doorStates)
        {
            currentMap = _currentMap;
            doorStates = _doorStates;
        }
    }
    [System.Serializable]
    public class Door
    {
        public int doorId;
        public bool doorState;
        public Door(bool _doorState, int _doorId)
        {
            doorId = _doorId;
            doorState = _doorState;
        }
    }
    [System.Serializable]
    public class Weapon
    {
        public string name;
        public int damage;
        public int magSize;
        public int currentAmmo;
        public float range;

        public Weapon(string _name, int _damage, int _magSize, int _currAmmo, float _range)
        {
            name = _name;
            damage = _damage;
            magSize = _magSize;
            currentAmmo = _currAmmo;
            range = _range;
        }
    }
    [System.Serializable]
    public class Player
    {
        public int health;
        public int armor;
        public int itemCount;
        public string[] inventory;

        public Player(int health, int armor, int itemCount, string[] inventory)
        {
            this.health = health;
            this.armor = armor;
            this.itemCount = itemCount;
            this.inventory = inventory;
        }
        public Player() {}
    }
    [System.Serializable]
    public class Enemy
    {
        public string name;
        public Vector3 position;
        public Quaternion rotation;
        public int health;
        public string animType;
        public float animState;

        public Enemy(string name, Vector3 position, Quaternion rotation, int health, string animType, float animState)
        {
            this.name = name;
            this.position = position;
            this.rotation = rotation;
            this.health = health;
            this.animType = animType;
            this.animState = animState;
        }
    }
    [System.Serializable]
    public class SaveSate
    {
        public List<Weapon> AllWeapons = new List<Weapon>();
        public List<Enemy> AllEnemies = new List<Enemy>();
        //public List<Door> Doors = new List<Door>();
        public LevelState LevelState = new LevelState();
        public Player PlayerState = new Player();
    }
}
