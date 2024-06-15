using System.Collections;
using UnityEngine;
public class Player : MonoBehaviour {
    public float jumpForce;
    public float moveSpeed;
    [Range(0, 2)]
    public float moveSpeedForwardModifier, moveSpeedBackwardModifier, moveSpeedLeftModifier, moveSpeedRightModifier;
    public float health;
    public float hunger;
    public int hungerDepletionRate;
    public int selectedItemInInventory;
    public GameObject playerInGameWorld;
    public new Rigidbody rigidbody;
    public void Start() {
        rigidbody = playerInGameWorld.GetComponent<Rigidbody>();
        StartCoroutine(DepleteHunger(1f));
    }
    public void MoveForward() {
        rigidbody.AddRelativeForce(new Vector3(moveSpeedForwardModifier * moveSpeed, 0, 0));
    }
    public void MoveBackward() {
        rigidbody.AddRelativeForce(new Vector3(-moveSpeedBackwardModifier * moveSpeed, 0, 0));
    }
    public void MoveLeft() {
        rigidbody.AddRelativeForce(new Vector3(0, 0, -moveSpeedLeftModifier * moveSpeed));
    }
    public void MoveRight() {
        rigidbody.AddRelativeForce(new Vector3(0, 0, moveSpeedRightModifier * moveSpeed));
    }
    public void Jump() {
        rigidbody.AddRelativeForce(new Vector3(0, jumpForce, 0));
    }
    public IEnumerator DepleteHunger(float time) {
        while (true) {
            yield return new WaitForSeconds(time);
            ModifyHunger(hungerDepletionRate);
        }
    }
    public void ModifyHunger(int amount) {
        hunger += amount;
        if(hunger < 1) {
            hunger = 0;
            ModifyHealth(-1);
        }
    }
    public void ModifyHealth(int amount) {
        health += amount;
        if(health < 1) {
            Debug.Log("Ur bloody ded!");
        }
    }
}
