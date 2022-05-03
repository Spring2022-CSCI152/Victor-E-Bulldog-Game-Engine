using System.Numerics;
using Bulldog.Utils.Collision;

// ReSharper disable InconsistentNaming
// ReSharper disable  CheckNamespace
// adapted from David W. Jeske's simpleScene

namespace Bulldog.Utils.BVH
{
    public class BVH<TObj>
    {
        public BVHNode<TObj> rootBVH;
        public IBVHNodeAdaptor<TObj> nAda;
        public readonly int LEAF_OBJ_MAX;
        public int nodeCount = 0;
        public int maxDepth = 0;
        
        public HashSet<BVHNode<TObj>> refitNodes = new HashSet<BVHNode<TObj>>();

        public delegate bool NodeTest(AABB box);
        
        public BVH()
        {
            
        }
        
        private void _traverse(BVHNode<TObj>? curNode, NodeTest hitTest, List<BVHNode<TObj>> hitList) {
            if (curNode == null) { return; }
            if (hitTest(curNode.Box)) {
                hitList.Add(curNode);
                _traverse(curNode.Left,hitTest,hitList);
                _traverse(curNode.Right,hitTest,hitList);
            }
        }

        public void addObject(TObj newOb) {
            AABB box = AABB.FromSphere(nAda.objectPos(newOb),nAda.radius(newOb));
            float boxSAH = BVHNode<TObj>.SA(ref box);
            rootBVH.addObject(nAda,newOb, ref box, boxSAH);
        }

        public void removeObject(TObj newObj) {
            var leaf = nAda.getLeaf(newObj);
            leaf.removeObject(nAda,newObj);
        }

        public int countBVHNodes() {
            return rootBVH.countBVHNodes();
        }
        
        public BVH(IBVHNodeAdaptor<TObj> nodeAdaptor, List<TObj> objects, int LEAF_OBJ_MAX = 1) {
            this.LEAF_OBJ_MAX = LEAF_OBJ_MAX;
            nodeAdaptor.setBVH(this);
            this.nAda = nodeAdaptor;
            
            if (objects.Count > 0) {
                rootBVH = new BVHNode<TObj>(this,objects);            
            } else {                
                rootBVH = new BVHNode<TObj>(this)
                {
                    Objects = new List<TObj>() // it's a leaf, so give it an empty object list
                };
            }
        }       
        
        
    }
}
