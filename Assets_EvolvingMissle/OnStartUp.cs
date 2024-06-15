using UnityEngine;

public class OnStartUp : MonoBehaviour
{
    void Start()
    {
        MissileBehaviour.antiMissileManager = GetComponent<AntiMissileManager>();
    }
}
