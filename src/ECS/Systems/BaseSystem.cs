using System.Numerics;

namespace Bulldog.ECS.Systems
{
    public abstract class BaseSystem : ECSObject {
        private readonly string[] RequireComponentNames;
        protected Entity CurrentEntity { get; set; }

        public BaseSystem(params string[] requireComponentNames) {
            RequireComponentNames = requireComponentNames;
        }

        protected T Get<T>() where T : Components.Component {
            return CurrentEntity.GetComponent<T>();
        }

        public bool Match(Entity entity) {
            if(RequireComponentNames.Length < 1)
                return false;
            foreach(string s in RequireComponentNames) {
                if(!entity.HasComponent(s)) return false;
            }

            CurrentEntity = entity;
            return true;
        }

        public virtual void Init() { }
        public virtual void Update(Vector3 vector3) { }
        public virtual void Draw() { }

        public override string ToString() {
            return string.Format("System<{0}:{1}>", Name, UID);
        }
    }
}