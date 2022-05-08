using Bulldog.ECS;
using Bulldog.ECS.Componets;
using Bulldog.ECS.Systems;

namespace Bulldog.test.ECSTests;
using Xunit;

public class WorldTest
{
    private readonly World _sut;
    private readonly List<Entity> _entities;
    private readonly BaseSystem[] _systems;
    private readonly TransformSystem _s;
    private readonly TransformSystem _s2;

    
    private readonly Entity _e;
    private readonly Entity _e2;

    private readonly Component[] _components; //This should be a mesh

    private readonly Component _c = null;

    // private readonly Mesh _c;

    public WorldTest()
    {
        //_c.Name = "TestC";
        _sut = new World();
        
        _entities = new List<Entity>();
        _systems = new BaseSystem[2];
        
        _e = new Entity("myName");
        _e2 = new Entity("myName2");
       
        //_system = new TransformSystem

        _entities.Add(_e);
        _entities.Add(_e2);

        //_s.Name = "Test1";
        //  _s2.Name = "Test2";
        
        //  _systems[0] = _s;
        // _systems[1] = _s2;


        _components = new Component[2];
        _components.SetValue(_c, 0);
        // _c = new Mesh();
    }

    [Fact(Skip = "Object reference not set to an instance of an object. This is because of Abstract class and our working functions do not work with derived classes  ")]
    public void TestCreateEntity()
    {
       // Component c = null;
       // c.Name = "TestC";
      //  Assert.NotNull(_c);
        Assert.Empty(_entities);      
        _sut.CreateEntity("TestE", _components); //cant shove anything into _components, and cant stub since its an abstract
        Assert.NotEmpty(_sut.Entities);
    }

    [Fact]
    public void TestAddEntity()
    {
        Assert.Empty(_sut.Entities);
        _sut.AddEntity(_e);
        Assert.Contains(_e, _sut.Entities);
    }
    
    [Fact]
    public void TestAddEntities()
    {
        Assert.Empty(_sut.Entities); 
        _sut.AddEntities(_entities);
        Assert.Equal(_e, _sut.Entities[0]);
        Assert.Equal(_e2, _sut.Entities[1]);
    }

    [Fact]
    public void TestRemoveEntity()
    {
        _sut.AddEntity(_e);
        Assert.Contains(_e, _sut.Entities);
        _sut.RemoveEntity(_e);
        Assert.DoesNotContain(_e, _sut.Entities);
    }

    [Fact(Skip = "Object reference not set to an instance of an object. This is because of Abstract class and our working function parameters do not work with derived classes  ")]
    public void TestAddSystem()
    {
        Assert.Empty(_sut.Systems);
        _sut.AddSystem(_s);
        Assert.NotEmpty(_sut.Systems);
    }
    
    [Fact(Skip = "Object reference not set to an instance of an object. This is because of Abstract class and our working function parameters do not work with derived classes  ")]
    public void TestAddSystems()
    {
        Assert.Empty(_sut.Systems);
        _sut.AddSystems(_systems);
        Assert.Contains(_s, _sut.Systems);
        Assert.Contains(_s2, _sut.Systems);

    }
    
    [Fact(Skip = "Object reference not set to an instance of an object. This is because of Abstract class and our working function parameters do not work with derived classes  ")]
    public void TestRemoveSystems()
    {
        Assert.Empty(_sut.Systems);
        _sut.AddSystem(_s);
        Assert.NotEmpty(_sut.Systems);
       // _sut.RemoveEntity(_s);
       Assert.Empty(_sut.Systems);
    }
    
}