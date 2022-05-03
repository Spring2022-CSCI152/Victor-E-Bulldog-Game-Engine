
#nullable enable
using Bulldog.Core;
using System.Numerics;
using Bulldog.Utils.Collision;

// ReSharper disable MemberCanBePrivate.Global

namespace Bulldog.Utils.BVH;

// ReSharper disable once InconsistentNaming
public class BVHNode<TObj>
{
    #region Member Variables

    public AABB Box;

    public BVHNode<TObj>? Parent;
    public BVHNode<TObj>? Left;
    public BVHNode<TObj>? Right;

    public int Depth;
    public int NodeNumber; // for debugging

    public List<TObj>? Objects;

    #endregion

    public bool IsLeaf
    {
        get
        {
            var isLeaf = Objects!.Any();
            if (isLeaf && ((Right != null) || (Left != null)))
            {
                throw new Exception("BVH Leaf has objects and left/right pointers!");
            }

            return isLeaf;
        }
    }
    
    void SetDepth(IBVHNodeAdaptor<TObj> nAda, int newdepth) {
        this.Depth = newdepth;
        if (newdepth > nAda.BVH.maxDepth) {
            nAda.BVH.maxDepth = newdepth;
        }
        if (Objects == null) {
            Left!.SetDepth(nAda, newdepth+1);
            Right!.SetDepth(nAda, newdepth+1);
        }
    }
    internal void addObject(IBVHNodeAdaptor<TObj> nAda, TObj newOb, ref AABB newObBox, float newObSAH) { 
        addObject(nAda,this,newOb, ref newObBox, newObSAH);
    }
    
    internal static AABB AABBofPair(BVHNode<TObj> nodea, BVHNode<TObj> nodeb) {
        AABB box = nodea.Box;
        box.ExpandToFit(nodeb.Box);
        return box;
    }
    internal static void addObject(IBVHNodeAdaptor<TObj> nAda, BVHNode<TObj> curNode, TObj newOb, ref AABB newObBox, float newObSah) { 
        // 1. first we traverse the node looking for the best leaf
        while (curNode.Objects == null) {
            // find the best way to add this object.. 3 options..
            // 1. send to left node  (L+N,R)
            // 2. send to right node (L,R+N)
            // 3. merge and pushdown left-and-right node (L+R,N)

            var left = curNode.Left;
            var right = curNode.Left;

            float leftSAH = SA(left!);
            float rightSAH = SA(right!);
            float sendLeftSAH = rightSAH + SA(left!.Box.ExpandedBy(newObBox));    // (L+N,R)
            float sendRightSAH = leftSAH + SA(right!.Box.ExpandedBy(newObBox));   // (L,R+N)
            float mergedLeftAndRightSAH = SA(AABBofPair(left,right)) + newObSah; // (L+R,N)

            // Doing a merge-and-pushdown can be expensive, so we only do it if it's notably better
            const float MERGE_DISCOUNT = 0.3f; 

            if (mergedLeftAndRightSAH < ( Math.Min(sendLeftSAH,sendRightSAH)) * MERGE_DISCOUNT ) {
                addObject_Pushdown(nAda,curNode,newOb);
                return;
            } else {
                if ( sendLeftSAH < sendRightSAH ) {                        
                    curNode = left;                        
                } else {                        
                    curNode = right;                        
                }
            }
        }
            
        // 2. then we add the object and map it to our leaf
        curNode.Objects.Add(newOb);
        nAda.mapObjectToBVHLeaf(newOb,curNode);                
        curNode.RefitVolume(nAda);
        // split if necessary...
        curNode.SplitIfNecessary(nAda);                    
    }
    
    internal static void addObject_Pushdown(IBVHNodeAdaptor<TObj> nAda, BVHNode<TObj> curNode, TObj newOb) {
        var left = curNode.Left;
        var right = curNode.Right;

        // merge and pushdown left and right as a new node..
        var mergedSubnode = new BVHNode<TObj>(nAda.BVH);
        mergedSubnode.Left = left;
        mergedSubnode.Right = right;
        mergedSubnode.Parent = curNode;
        mergedSubnode.Objects = null; // we need to be an interior node... so null out our object list..
        left!.Parent = mergedSubnode;
        right!.Parent = mergedSubnode;
        mergedSubnode.ChildRefit(nAda, propagate: false);

        // make new subnode for obj
        var newSubnode = new BVHNode<TObj>(nAda.BVH);
        newSubnode.Parent = curNode;
        newSubnode.Objects = new List<TObj> { newOb };
        nAda.mapObjectToBVHLeaf(newOb, newSubnode);
        newSubnode.ComputeVolume(nAda);

        // make assignments..
        curNode.Left = mergedSubnode;
        curNode.Right = newSubnode;
        curNode.SetDepth(nAda, curNode.Depth); // propagate new depths to our children.
        curNode.ChildRefit(nAda);                  
    }
    
    internal int countBVHNodes()
    {
        if (Objects != null) {
            return 1;
        }
        return Left!.countBVHNodes() + Right!.countBVHNodes();
    }

    private Axis PickSplitAxis()
    {
        var axisX = Box.Max.X - Box.Min.X;
        var axisY = Box.Max.Y - Box.Min.Y;
        var axisZ = Box.Max.Z - Box.Min.Z;

        // return the biggest axis
        if (axisX > axisY)
        {
            return axisX > axisZ ? Axis.X : Axis.Z;
        }

        return axisY > axisZ ? Axis.Y : Axis.Z;
    }

    private static Axis NextAxis(Axis cur)
    {
        return cur switch
        {
            Axis.X => Axis.Y,
            Axis.Y => Axis.Z,
            Axis.Z => Axis.X,
            _ => throw new NotSupportedException()
        };
    }

    private void ExpandVolume(IBVHNodeAdaptor<TObj> nAda, Vector3 pos, float radius)
    {
        bool expanded = false;

        // test min X and max X against the current bounding volume
        if ((pos.X - radius) < Box.Min.X)
        {
            Box.Min.X = (pos.X - radius);
            expanded = true;
        }

        if ((pos.X + radius) > Box.Max.X)
        {
            Box.Max.X = (pos.X + radius);
            expanded = true;
        }

        // test min Y and max Y against the current bounding volume
        if ((pos.Y - radius) < Box.Min.Y)
        {
            Box.Min.Y = (pos.Y - radius);
            expanded = true;
        }

        if ((pos.Y + radius) > Box.Max.Y)
        {
            Box.Max.Y = (pos.Y + radius);
            expanded = true;
        }

        // test min Z and max Z against the current bounding volume
        if ((pos.Z - radius) < Box.Min.Z)
        {
            Box.Min.Z = (pos.Z - radius);
            expanded = true;
        }

        if ((pos.Z + radius) > Box.Max.Z)
        {
            Box.Max.Z = (pos.Z + radius);
            expanded = true;
        }

        if (expanded && Parent != null)
        {
            Parent.ChildExpanded(nAda, this);
        }
    }

    private void AssignVolume(Vector3 pos, float radius)
    {
        Box.Min.X = pos.X - radius;
        Box.Max.X = pos.X + radius;
        Box.Min.Y = pos.Y - radius;
        Box.Max.Y = pos.Y + radius;
        Box.Min.Z = pos.Z - radius;
        Box.Max.Z = pos.Z + radius;
    }

    internal void ComputeVolume(IBVHNodeAdaptor<TObj> nAda)
    {
        AssignVolume(nAda.objectPos(Objects![0]), nAda.radius(Objects[0]));
        for (int i = 1; i < Objects.Count; i++)
        {
            ExpandVolume(nAda, nAda.objectPos(Objects[i]), nAda.radius(Objects[i]));
        }
    }

    private static List<Axis> EachAxis => new List<Axis>((Axis[]) Enum.GetValues(typeof(Axis)));

    internal class SplitAxisOpt<TObj> : IComparable<SplitAxisOpt<TObj>>
    {
        // split Axis option
        public float SAH;
        public Axis axis;
        public List<TObj> left, right;

        internal SplitAxisOpt(float sah, Axis axis, List<TObj> left, List<TObj> right)
        {
            this.SAH = sah;
            this.axis = axis;
            this.left = left;
            this.right = right;
        }

        public int CompareTo(SplitAxisOpt<TObj>? other)
        {
            return SAH.CompareTo(other!.SAH);
        }
    }

    internal static float SA(AABB box) {
        float x_size = box.Max.X - box.Min.X;
        float y_size = box.Max.Y - box.Min.Y;
        float z_size = box.Max.Z - box.Min.Z;

        return 2.0f * ( (x_size * y_size) + (x_size * z_size) + (y_size * z_size) );
            
    }
    
    internal static float SA(ref AABB box) {
        float x_size = box.Max.X - box.Min.X;
        float y_size = box.Max.Y - box.Min.Y;
        float z_size = box.Max.Z - box.Min.Z;

        return 2.0f * ( (x_size * y_size) + (x_size * z_size) + (y_size * z_size) );
            
    }
    internal static float SA(BVHNode<TObj> node) {            
        float x_size = node.Box.Max.X - node.Box.Min.X;
        float y_size = node.Box.Max.Y - node.Box.Min.Y;
        float z_size = node.Box.Max.Z - node.Box.Min.Z;

        return 2.0f * ( (x_size * y_size) + (x_size * z_size) + (y_size * z_size) );
    }
    internal static float SA(IBVHNodeAdaptor<TObj> nAda, TObj obj) {            
        float radius = nAda.radius(obj);

        float size = radius * 2;
        return 6.0f * (size * size);            
    }
    internal static AABB AABBofOBJ(IBVHNodeAdaptor<TObj> nAda, TObj obj)
    {
        float radius = nAda.radius(obj);
        AABB box = new AABB();
        box.Min.X = -radius; box.Max.X = radius;
        box.Min.Y = -radius; box.Max.Y = radius;
        box.Min.Z = -radius; box.Max.Z = radius;
        return box;
    }

    internal float SAofList(IBVHNodeAdaptor<TObj> nAda, List<TObj> list) {
        var box = AABBofOBJ(nAda,list[0]);

        List<TObj> list1 = new List<TObj>();
        foreach (var obj in list) list1.Add(obj);
        list1.GetRange(1,list.Count-1).ForEach( obj => {
            var newbox = AABBofOBJ(nAda, obj);
            box.ExpandBy(newbox);
        });
        return SA(box);
    }

    internal void SplitNode(IBVHNodeAdaptor<TObj> nAda)
    {
        // second, decide which axis to split on, and sort..
        List<TObj> splitList = Objects!;
        splitList.ForEach(o => nAda.unmapObject(o));
        int center = (splitList.Count / 2); // find the center object

        SplitAxisOpt<TObj> bestSplit = EachAxis.Min((axis) =>
        {
            var orderedList = new List<TObj>(splitList);
            switch (axis)
            {
                case Axis.X:
                    orderedList.Sort((go1, go2) => nAda.objectPos(go1).X.CompareTo(nAda.objectPos(go2).X));
                    break;
                case Axis.Y:
                    orderedList.Sort((go1, go2) => nAda.objectPos(go1).Y.CompareTo(nAda.objectPos(go2).Y));
                    break;
                case Axis.Z:
                    orderedList.Sort((go1, go2) => nAda.objectPos(go1).Z.CompareTo(nAda.objectPos(go2).Z));
                    break;
                default:
                    throw new NotImplementedException("unknown split axis: " + axis.ToString());
            }

            var leftS = orderedList.GetRange(0, center);
            var rightS = orderedList.GetRange(center, splitList.Count - center);

            float sah = SAofList(nAda, leftS) * leftS.Count + SAofList(nAda, rightS) * rightS.Count;
            return new SplitAxisOpt<TObj>(sah, axis, leftS, rightS);
        })!;

        // perform the split
        Objects = null;
        this.Left = new BVHNode<TObj>(nAda.BVH, this, bestSplit!.left, bestSplit.axis,
            this.Depth + 1); // Split the Hierarchy to the left
        this.Right =
            new BVHNode<TObj>(nAda.BVH, this, bestSplit.right, bestSplit.axis,
                this.Depth + 1); // Split the Hierarchy to the right                                
    }

    internal void SplitIfNecessary(IBVHNodeAdaptor<TObj> nAda)
    {
        if (Objects!.Count > nAda.BVH.LEAF_OBJ_MAX)
        {
            SplitNode(nAda);
        }
    }

    internal bool RefitVolume(IBVHNodeAdaptor<TObj> nAda)
    {
        if (Objects!.Count == 0)
        {
            throw new NotImplementedException();
        } // 
        
            AABB oldBox = Box;

            ComputeVolume(nAda);
            if (!Box.Equals(oldBox))
            {
                if (Parent != null) Parent.ChildRefit(nAda);
                return true;
            }
            else
            {
                return false;
            }
            
    }

    internal void removeObject(IBVHNodeAdaptor<TObj> nAda, TObj newOb) {
        if (Objects == null) { throw new Exception("removeObject() called on nonLeaf!"); }

        nAda.unmapObject(newOb);
        Objects.Remove(newOb);
        if (Objects.Count > 0) {
            RefitVolume(nAda);
        } else {
            // our leaf is empty, so collapse it if we are not the root...
            if (Parent != null) {
                Objects = null;
                Parent.RemoveLeaf(nAda, this);
                Parent = null;
            } 
        }
    }

    internal void RemoveLeaf(IBVHNodeAdaptor<TObj> nAda, BVHNode<TObj> removeLeaf)
    {
        if (Left == null || Right == null)
        {
            throw new Exception("bad intermediate node");
        }

        BVHNode<TObj> keepLeaf;

        if (removeLeaf == Left)
        {
            keepLeaf = Right;
        }
        else if (removeLeaf == Right)
        {
            keepLeaf = Left;
        }
        else
        {
            throw new Exception("removeLeaf doesn't match any leaf!");
        }
    }

    internal void ChildExpanded(IBVHNodeAdaptor<TObj> nAda, BVHNode<TObj> child)
    {
        bool expanded = false;

        if (child.Box.Min.X < Box.Min.X)
        {
            Box.Min.X = child.Box.Min.X;
            expanded = true;
        }

        if (child.Box.Max.X > Box.Max.X)
        {
            Box.Max.X = child.Box.Max.X;
            expanded = true;
        }

        if (child.Box.Min.Y < Box.Min.Y)
        {
            Box.Min.Y = child.Box.Min.Y;
            expanded = true;
        }

        if (child.Box.Max.Y > Box.Max.Y)
        {
            Box.Max.Y = child.Box.Max.Y;
            expanded = true;
        }

        if (child.Box.Min.Z < Box.Min.Z)
        {
            Box.Min.Z = child.Box.Min.Z;
            expanded = true;
        }

        if (child.Box.Max.Z > Box.Max.Z)
        {
            Box.Max.Z = child.Box.Max.Z;
            expanded = true;
        }

        if (expanded && Parent != null)
        {
            Parent.ChildExpanded(nAda, this);
        }
    }

    internal void ChildRefit(IBVHNodeAdaptor<TObj> nAda, bool propagate = true)
    {
        ChildRefit(nAda, this, propagate: propagate);
    }

    internal static void ChildRefit(IBVHNodeAdaptor<TObj> nAda, BVHNode<TObj> curNode, bool propagate = true)
    {
        do
        {
            AABB oldBox = curNode.Box;
            BVHNode<TObj> left = curNode.Left!;
            BVHNode<TObj> right = curNode.Right!;

            // start with the left box
            AABB newBox = left.Box;

            // expand any dimension bigger in the right node
            if (right.Box.Min.X < newBox.Min.X)
            {
                newBox.Min.X = right.Box.Min.X;
            }

            if (right.Box.Min.Y < newBox.Min.Y)
            {
                newBox.Min.Y = right.Box.Min.Y;
            }

            if (right.Box.Min.Z < newBox.Min.Z)
            {
                newBox.Min.Z = right.Box.Min.Z;
            }

            if (right.Box.Max.X > newBox.Max.X)
            {
                newBox.Max.X = right.Box.Max.X;
            }

            if (right.Box.Max.Y > newBox.Max.Y)
            {
                newBox.Max.Y = right.Box.Max.Y;
            }

            if (right.Box.Max.Z > newBox.Max.Z)
            {
                newBox.Max.Z = right.Box.Max.Z;
            }

            // now set our box to the newly created box
            curNode.Box = newBox;

            // and walk up the tree
            if (curNode.Parent != null) curNode = curNode.Parent;
        } while (propagate && curNode != null);
    }

    internal BVHNode(BVH<TObj> bvh)
    {
        Objects = new List<TObj>();
        Left = Right = null;
        Parent = null;
    }

    internal BVHNode(BVH<TObj> bvh, List<TObj> objectList) : this(bvh, null!, objectList, Axis.X, 0)
    {
    }

    private BVHNode(BVH<TObj> bvh, BVHNode<TObj> lParent, List<TObj> objectList, Axis lastSplitAxis, int curDepth)
    {
        IBVHNodeAdaptor<TObj> nAda = bvh.nAda;

        this.Parent = lParent; // save off the parent BVHObj Node

        if (bvh.maxDepth < curDepth)
        {
            bvh.maxDepth = curDepth;
        }

        // Early out check due to bad data
        // If the list is empty then we have no BVHObj, or invalid parameters are passed in
        if (objectList == null || objectList.Count < 1)
        {
            throw new Exception("ssBVHNode constructed with invalid parameters");
        }

        // Check if we’re at our LEAF node, and if so, save the objects and stop.  Also store the min/max for the leaf node and update the parent appropriately
        if (objectList.Count <= bvh.LEAF_OBJ_MAX)
        {
            // once we reach the leaf node, we must set prev/next to null to signify the end
            Left = null;
            Right = null;
            // at the leaf node we store the remaining objects, so initialize a list
            Objects = objectList;
            Objects.ForEach(o => nAda.mapObjectToBVHLeaf(o, this));
            ComputeVolume(nAda);
            SplitIfNecessary(nAda);
        }
        else
        {
            // --------------------------------------------------------------------------------------------
            // if we have more than (bvh.LEAF_OBJECT_COUNT) objects, then compute the volume and split
            Objects = objectList;
            ComputeVolume(nAda);
            SplitNode(nAda);
            ChildRefit(nAda, propagate: false);
        }

    }
}