using System;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace BlockBehaviors
{
    public class IceBlockBehavior : BlockBehavior
    {
        [SerializeField] private ParticleSystem fx;
        [SerializeField] private MeshRenderer mesh;
        [Space]
        [SerializeField] AudioClip breakSound;

        public override bool TryMoveOn(IBoardable move, Enums.Side commingSide, Vector3Int pos)
        {
            List<Enums.BlockTag> moverTags = move.GetTags();
            if (moverTags.Contains(Enums.BlockTag.Penguin))
            {
                boardMaster.RemoveBoardable(this);
                mesh.enabled = false;
                fx.Play();
                m_Audio.PlaySound(breakSound);
                
                //Destroy(gameObject);
                
                return true;
            }
            return false;
        }

        private void OnParticleSystemStopped()
        {
            Debug.Log("Destroy Ice");
            Destroy(gameObject);
        }
    }
}