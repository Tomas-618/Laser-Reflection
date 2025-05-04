using Source.Extensions;
using UnityEngine;

namespace Source.Components
{
    public class Example : MonoBehaviour
    {
        [SerializeField] private Transform _child;

        private void OnDrawGizmos()
        {
            var transformData = transform.ToData();

            Debug.Log(transformData.InverseTransformPoint(_child.position));
        }
    }
}
