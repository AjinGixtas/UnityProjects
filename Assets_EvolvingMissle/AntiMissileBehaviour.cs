using UnityEngine;

public class AntiMissileBehaviour : MonoBehaviour {
    public enum DestructReason { TARGET_ELIMINATED = 0, FUEL_DEPLETED = 1};
    public static AntiMissileManager antiMissileManager;
    public float currentFuel;
    public float currentMaxSpeed;
    public GameObject target;
    public Vector2 velocity = new(0, 0);
    public DNA dna;
    public float fuelCapacityPoint, centeringFactorPoint, accelerationPoint, minSpeedPoint, maxSpeedPoint, accuracyFactorPoint;
    public float fuelCapacity, centeringFactor, acceleration, minSpeed, maxSpeed, accuracyFactor;
    public const float 
        fuelCapacityMultiplier = 2,
        centeringFactorMultiplier = 0.1f, 
        accelerationMultiplier = 0.75f, 
        minSpeedMultiplier = 1f, 
        maxSpeedMultiplier = 6.5f,
        accuracyMultiplier = 8;
    public void SetStat(DNA dna) {
        this.dna = dna;
        fuelCapacity = dna.fuelCapacity * fuelCapacityMultiplier;
        centeringFactor = dna.centeringFactor * centeringFactorMultiplier;
        acceleration = dna.acceleration * accelerationMultiplier;
        minSpeed = dna.minSpeed * minSpeedMultiplier;
        maxSpeed = minSpeed + dna.maxSpeed * maxSpeedMultiplier;
        accuracyFactor = accuracyMultiplier - dna.accuracy * accuracyMultiplier < 0 
            ? 0 : accuracyMultiplier - dna.accuracy * accuracyMultiplier;
        currentFuel = fuelCapacity;
        currentMaxSpeed = minSpeed;
        fuelCapacityPoint = dna.fuelCapacity;
        centeringFactorPoint = dna.centeringFactor;
        accelerationPoint = dna.acceleration;
        minSpeedPoint = dna.minSpeed;
        maxSpeedPoint = dna.maxSpeed;
        accuracyFactorPoint = dna.accuracy;
    }
    float angle;
    private void Update() {
        if(target == null) { target = antiMissileManager.GetTargetMissile(); }
        if (target != null) { CalculateVelocity(); } else if(accuracyFactor != 0) { AddNoisyVelocity(); }
        SetVelocity();
        if (!float.IsNaN(velocity.x)) transform.position +=(Vector3)velocity * Time.deltaTime;
        else velocity = new(0, 0);
        currentFuel -= Time.deltaTime;
        currentMaxSpeed += acceleration * Time.deltaTime;
        if(currentFuel <= 0) { SelfDestruct(DestructReason.FUEL_DEPLETED); }
        angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    Vector2 accelerationVelocity;
    public void CalculateVelocity() {
        accelerationVelocity = (target.transform.position - transform.position) * centeringFactor +
            new Vector3(Random.Range(-accuracyFactor, accuracyFactor), Random.Range(-accuracyFactor, accuracyFactor));
    }
    public void AddNoisyVelocity() {
        accelerationVelocity = new Vector2(Random.Range(-accuracyFactor, accuracyFactor), Random.Range(-accuracyFactor, accuracyFactor));
    }
    float currentSpeed = 0;
    public void SetVelocity() {
        velocity += accelerationVelocity;
        currentSpeed = Mathf.Sqrt(Mathf.Pow(velocity.x, 2) + Mathf.Pow(velocity.y, 2));
        currentMaxSpeed += acceleration;
        if(currentMaxSpeed > maxSpeed) { currentMaxSpeed = maxSpeed; }
        if (currentSpeed > currentMaxSpeed) { velocity = velocity / currentSpeed * currentMaxSpeed; } 
        else if (currentSpeed < minSpeed) { velocity = velocity / currentSpeed * minSpeed; }
    }
    public void SelfDestruct(DestructReason reason) {
        antiMissileManager.NotifyDestruction(dna, reason);
        Destroy(this.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Missile")) {
            collision.gameObject.GetComponent<MissileBehaviour>().SelfDestruct();
            SelfDestruct(DestructReason.TARGET_ELIMINATED);
        }
    }
}

