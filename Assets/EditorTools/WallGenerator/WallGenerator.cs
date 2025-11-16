using UnityEngine;

namespace EditorTools.WallGenerator
{
    [ExecuteInEditMode]
    public class WallGenerator : MonoBehaviour
    {
        [Header("Appearance")] 
        [Tooltip("Assign a Material to apply to all wall blocks.")]
        public Material wallMaterial;

        [Header("Enclosure Dimensions")]
        public float wallWidth = 50f;
        public float wallDepth = 50f;

        [Header("Block Size & Randomization")]
        [Tooltip("The MIN/MAX scale for a block's Width (X), Height (Y), and Depth (Z).")]
        public Vector3 minScale = new(1f, 2f, 1f);

        public Vector3 maxScale = new(4f, 5f, 2f);

        [Tooltip("Random Y-axis rotation applied to each block for variation.")]
        public float randomYRotationRange = 15f;

        public void GenerateWall()
        {
            ClearWall();

            var center = transform.position;
            var halfWidth = wallWidth / 2f;
            var halfDepth = wallDepth / 2f;

            var c0 = center + new Vector3(-halfWidth, 0, -halfDepth); // Bottom-Left
            var c1 = center + new Vector3(halfWidth, 0, -halfDepth); // Bottom-Right
            var c2 = center + new Vector3(halfWidth, 0, halfDepth); // Top-Right
            var c3 = center + new Vector3(-halfWidth, 0, halfDepth); // Top-Left

            GenerateSide(c0, c1, 1);
            GenerateSide(c1, c2, 2);
            GenerateSide(c3, c2, 3);
            GenerateSide(c0, c3, 4);
        }

        public void ClearWall()
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
        
        private void GenerateSide(Vector3 startPoint, Vector3 endPoint, int sideIndex)
        {
            var direction = (endPoint - startPoint).normalized;
            var currentPlacementPoint = startPoint;
            var sideLength = Vector3.Distance(startPoint, endPoint);
            var distanceCovered = 0f;
            var blockIndex = 0;
            
            var baseAlignmentRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -90f, 0);

            while (distanceCovered < sideLength)
            {
                blockIndex++;
                
                // Create a simple cube
                var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                block.name = $"Wall_Side{sideIndex}_Block{blockIndex}";
                block.transform.SetParent(transform);

                // Apply the material
                var blockRenderer = block.GetComponent<MeshRenderer>();
                if (wallMaterial && blockRenderer)
                {
                    blockRenderer.material = wallMaterial;
                }
                
                // Scale randomly
                var randomScale = new Vector3(
                    Random.Range(minScale.x, maxScale.x),
                    Random.Range(minScale.y, maxScale.y),
                    Random.Range(minScale.z, maxScale.z)
                );
                block.transform.localScale = randomScale;
                var blockWidth = randomScale.x;

                // Calculate the base position on the YZ ground plane.
                var position = currentPlacementPoint + direction * (blockWidth / 2f);
                position.y += randomScale.y / 2f;
                block.transform.position = position;

                // Apply random Y rotation.
                var randomSpin = Quaternion.Euler(0, Random.Range(-randomYRotationRange, randomYRotationRange), 0);
                block.transform.rotation = baseAlignmentRotation * randomSpin;

                // Advance the placement point for the next block.
                currentPlacementPoint += direction * blockWidth;
                distanceCovered += blockWidth;
            }
        }
    }
}