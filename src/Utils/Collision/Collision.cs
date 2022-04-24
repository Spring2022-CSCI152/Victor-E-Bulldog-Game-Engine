namespace Bulldog.Utils.Collision;

public class Collision
{
    public Collision()
    {
    }

    public IBox Box { get; set; }

    public IBox Other => Hit?.Box;

    public IHit Hit { get; set; }

    public bool HasCollided => this.Hit != null;
}