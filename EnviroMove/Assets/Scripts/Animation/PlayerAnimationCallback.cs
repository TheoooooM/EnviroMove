using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationCallback : MonoBehaviour
{
    [SerializeField]private Player player;
    
    public void Die()
    {
        player.AsyncGameOver();
    }
}
