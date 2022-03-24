namespace Bulldog.ECS;
using System.Collections.Generic;
using System.Diagnostics;
// ReSharper disable once InconsistentNaming
public abstract class ECSObject {
    public readonly string UID;
    public string Name;

    public ECSObject() {
        UID = Helper.CreateUID();
        Name = GetType().Name;
    }
}

static class Helper {
    // ReSharper disable once InconsistentNaming
    public static string CreateUID() {
        return Guid.NewGuid().ToString("N");
    }
    // ReSharper disable once InconsistentNaming
    public static bool CheckExistUID<T, K>(List<T> list, T item, K owner) where T : ECSObject where K : ECSObject {
        if(list.Count == 0)
            return false;

        if(list.Exists(x => x.UID == item.UID)) {
            Debug.WriteLine(string.Format("UID | {0} already exist in {1}", item, owner));
            return true;
        }

        return false;
    }

    public static bool CheckExistName<T, K>(List<T> list, T item, K owner) where T : ECSObject where K : ECSObject {
        if(list.Count == 0)
            return false;

        if (!list.Exists(x => x.Name == item.Name)) return false;
        Debug.WriteLine(string.Format("Name | {0} already exist in {1}", item, owner));
        return true;

    }
}