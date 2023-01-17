using UnityEngine;

namespace Source
{
    public class ResourcePrefabProvider : IPrefabsProvider
    {
        public GameObject Instantiate(string path) =>
            Instantiate(path, Vector2.zero);

        public GameObject Instantiate(string path, Vector2 at) =>
            Object.Instantiate(Resources.Load<GameObject>(path), at, Quaternion.identity);
    }
}