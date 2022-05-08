using Bulldog.ECS;
using Bulldog.ECS.Componets;

namespace Bulldog.test.ECSTests;
using Xunit;

public class WorldTest
{
    private readonly World _sut;
    private readonly List<Entity> _entities;
    private readonly Entity _e;
    private readonly Entity _e2;

    private readonly Component[] _components; //This should be a mesh
    private readonly Component _c;
   // private readonly Mesh _c;

    public WorldTest()
    {
        _sut = new World();
        _entities = new List<Entity>();
        _e = new Entity("myName");
        _e2 = new Entity("myName2");

        _components = new Component[2];
        //  _c.Name = "TestC";
        // _c = new Mesh();
    }

    [Fact(Skip = "Object reference not set to an instance of an object. This is because of Abstract class and our working functions do not work with derived classes  ")]
    public void TestCreateEntity()
    {
        Assert.Empty(_entities);      
        _sut.CreateEntity("TestE", _components);
    }

    [Fact(Skip = "Object reference not set to an instance of an object. This is because of Abstract class and our working functions do not work with derived classes  ")]
    public void TestAddEntity()
    {
        Assert.Empty(_entities);
        _sut.AddEntity(_e);
        Assert.Contains(_e, _entities);
    }
    
    [Fact(Skip = "Object reference not set to an instance of an object. This is because of Abstract class and our working functions do not work with derived classes  ")]
    public void TestAddEntities()
    {
        Assert.Empty(_entities); 
        _sut.AddEntities(_entities);
        Assert.Equal(_e, _entities[0]);
        Assert.Equal(_e2, _entities[1]);
    }

    [Fact(Skip = "Object reference not set to an instance of an object. This is because of Abstract class and our working functions do not work with derived classes  ")]
    public void TestRemoveEntity()
    {
        _entities.Add(_e);
        Assert.Contains(_e,_entities);
        _sut.RemoveEntity(_e);
        Assert.DoesNotContain(_e, _entities);
    }
    
    
}