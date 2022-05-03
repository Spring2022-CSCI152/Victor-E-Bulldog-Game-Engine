using System.Numerics;

namespace Bulldog.Utils.BVH;
public interface IBVHNodeAdaptor<TObj>
{
    BVH<TObj> BVH { get; }
        
    Vector3 objectPos(TObj obj);
    float radius(TObj obj);
        
    void setBVH(BVH<TObj> bvh);
    void mapObjectToBVHLeaf(TObj obj, BVHNode<TObj> leaf);
    void unmapObject(TObj obj);
    void checkMap(TObj obj);
        
    BVHNode<TObj> getLeaf(TObj obj);
}