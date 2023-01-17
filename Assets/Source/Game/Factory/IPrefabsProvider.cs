using UnityEngine;

namespace Source
{
    internal interface IPrefabsProvider
    {
        GameObject Instantiate(string path);
        GameObject Instantiate(string path, Vector2 at);
    }
}