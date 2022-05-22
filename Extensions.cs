using Monocle.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Monocle
{
    internal static class Extensions
    {
        public static Position ToPosition(this Vector3 v)
        {
            return new Position
            {
                x = v.x,
                y = v.y,
                z = v.z,
            };
        }
    }
}
