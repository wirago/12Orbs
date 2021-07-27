namespace Orbs.Pathfinding
{
    public class Node
    {
        public bool walkable;
        public int gridX;
        public int gridY;
        public float price;

        // calculated values while finding path
        public int gCost;
        public int hCost;
        public Node parent;

        /// <summary>
        /// Create the grid node.
        /// </summary>
        /// <param name="price">Price to walk on this node (set to 1.0f to ignore).</param>
        /// <param name="gridX">Node x index.</param>
        /// <param name="gridY">Node y index.</param>
        public Node(float price, int gridX, int gridY)
        {
            walkable = price != 0.0f;
            this.price = price;
            this.gridX = gridX;
            this.gridY = gridY;
        }

        /// <summary>
        /// Create the grid node.
        /// </summary>
        /// <param name="walkable">Is this tile walkable?</param>
        /// <param name="gridX">Node x index.</param>
        /// <param name="gridY">Node y index.</param>
        public Node(bool walkable, int gridX, int gridY)
        {
            this.walkable = walkable;
            price = walkable ? 1f : 0f;
            this.gridX = gridX;
            this.gridY = gridY;
        }

        /// <summary>
        /// Get fCost of this node.
        /// </summary>
        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }
    }
}