using BlockBehaviors;
using UnityEngine;

namespace Animation
{
    public class RabbitAnimationCallbacks : MonoBehaviour
    {
        [SerializeField] private RabbitBehavior _rabbit;
        
        public void Dig()
        {
            _rabbit.CreateTunnel(_rabbit.tempPos);
        }
    }
}