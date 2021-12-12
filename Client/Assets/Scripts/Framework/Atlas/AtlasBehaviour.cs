/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/08/18 15:40:43
** desc:  图集集合;
*********************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace Framework.AtlasModule
{
    public class AtlasBehaviour : MonoBehaviour
    {
        [SerializeField]
        public List<Sprite> _spriteList = new List<Sprite>();
    }
}