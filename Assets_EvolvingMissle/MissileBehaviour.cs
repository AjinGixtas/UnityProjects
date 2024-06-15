using UnityEngine;

public class MissileBehaviour : MonoBehaviour {
    public static AntiMissileManager antiMissileManager;
    public float speed;
    void Awake() { 
        antiMissileManager.AddTargetMissile(this.gameObject); 
    }
    void Update() {
        transform.Translate(new Vector2(speed, 0) * Time.deltaTime);
        if(transform.position.y < -3) { SelfDestruct(); }
    }
    public void SelfDestruct() {
        antiMissileManager.RemoveTargetMissile(this.gameObject);
        Destroy(this.gameObject);
    }
}
