using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NullscapeinBB.Intermissions
{
    public class IntermissionMovementStuff : MonoBehaviour
    {
        public Vector3 StartPos;
        void Start() {
            StartPos = transform.position;
        }

        void Update() {
            transform.position = StartPos + new Vector3(Mathf.Sin(Time.time / 2.66654f) * 1.5f, Mathf.Cos(Time.time / (Mathf.PI/2f)) * .5f);
        }
    }
}
