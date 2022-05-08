using Bulldog.ECS.Systems;
namespace Bulldog.ECS {
    public class World : ECSObject {

        internal readonly List<Entity> Entities;
        internal readonly List<BaseSystem> Systems;

        public World() {
            Entities = new List<Entity>();
            Systems = new List<BaseSystem>();
        }
        
        public void CreateEntity(string name, params Component[] components) {
            Entity entity = new Entity(name);
            entity.AddComponents(components);
            AddEntity(entity);
        }

        // Entity Add/Remove //
        public void AddEntity(Entity entity) {
            if(Helper.CheckExistUID(Entities, entity, this))
                return;

            Entities.Add(entity);
            entity.World = this;
        }

        public void AddEntities(List<Entity> entities) {
            foreach(Entity entity in entities)
                AddEntity(entity);
        }

        public bool RemoveEntity(Entity entity) {
            return Entities.Remove(entity);
        }

        public bool RemoveEntity(string name) {
            return Entities.RemoveAll(x => x.Name == name) > 0;
        }

        // System Add/Remove //
        public void AddSystem(BaseSystem system) {
            if(Helper.CheckExistName(Systems, system, this))
                return;

            Systems.Add(system);
        }

        public void AddSystems(params BaseSystem[] systems) {
            foreach(BaseSystem system in systems)
                AddSystem(system);
        }

        public bool RemoveSystem(BaseSystem system) {
            return Systems.Remove(system);
        }

        public bool RemoveSystem(string name) {
            return Systems.RemoveAll(x => x.Name == name) > 0;
        }

        // System Loop //
        enum ForEachType { Init, Update, Draw }
        private void ForEach(ForEachType type, float dt) {
            Systems.ForEach(s => {
                Entities.ForEach(e => {
                    if(s.Match(e)) { // <- this set CurrentEntity
                        switch(type) {
                            case ForEachType.Init:
                                s.Init();
                                break;
                            case ForEachType.Update:
                                s.Update(dt);
                                break;
                            case ForEachType.Draw:
                                s.Draw();
                                break;
                        }
                    }
                });
            });
        }

        public void Init() {
            ForEach(ForEachType.Init, 0);
        }

        public void Update(float dt) {
            ForEach(ForEachType.Update, dt);
        }

        public void Draw() {
            ForEach(ForEachType.Draw, 0);
        }

        //▼▾ Override ToString  ▾▼//
        public override string ToString() {
            return string.Format("World<{0}:{1}>", Name, UID);
        }
    }
}