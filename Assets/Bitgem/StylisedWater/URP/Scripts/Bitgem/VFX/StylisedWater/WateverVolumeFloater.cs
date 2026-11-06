#region Using statements

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Bitgem.VFX.StylisedWater
{
    public class WateverVolumeFloater : MonoBehaviour
    {
        #region Public fields

        public WaterVolumeHelper WaterVolumeHelper = null;
        [SerializeField] private float queTanSumergido;
        public float altura;

        #endregion

        #region MonoBehaviour events

        void Update()
        {
            var instance = WaterVolumeHelper ? WaterVolumeHelper : WaterVolumeHelper.Instance;
            if (!instance)
            {
                return;
            }

            //transform.position = new Vector3(transform.position.x, (instance.GetHeight(transform.position) - queTanSumergido) ?? transform.position.y, transform.position.z);
            altura = (instance.GetHeight(transform.position) - queTanSumergido) ?? transform.position.y;
        }

        #endregion
    }
}