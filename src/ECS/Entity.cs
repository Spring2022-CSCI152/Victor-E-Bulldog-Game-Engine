using System.Diagnostics;
using Component = Bulldog.ECS.Components;
using Bulldog.ECS.Components;
namespace Bulldog.ECS {

    
    public class Entity : ECSObject {
        
        
        private readonly List<Components.Component> Components = new List<Components.Component>();
        internal World World;
        public Entity Parent;
        
        public Entity[] Children { get { if(World == null) return new Entity[0]; return World.Entities.FindAll(x => x.Parent == this).ToArray(); } }

        public Entity(){ }
        public Entity(string name) {
            Name = name;
        }

        public void AddComponent(Components.Component component) {
            if(Helper.CheckExistName(Components, component, this))
                return;

            Components.Add(component);
        }

        public void AddComponents(params Components.Component[] components) {
            foreach(Components.Component component in components)
                AddComponent(component);
        }

        public Components.Component GetComponent(string name){
            try {
                return Components.Find(x => x.Name == name);
            } catch(ArgumentNullException e) {
                Debug.WriteLine("Component not exist: " + e);
            }

            return null;
        }

        public T GetComponent<T>() where T : Components.Component {
            return (T)GetComponent(typeof(T).Name);
        }

        public bool HasComponent(string name) {
            return Components.Exists(x => x.Name == name);
        }
        
        public bool HasComponent<T>() where T : Components.Component {
            return HasComponent(typeof(T).Name);
        }

        public bool RemoveComponent(string name) {            
            return Components.RemoveAll(x => x.Name == name) > 0;
        }

        public override string ToString() {
            return string.Format("Entity<{0}:{1}>", Name, UID);
        }
    }
}