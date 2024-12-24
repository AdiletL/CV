using System.Collections.Generic;

public interface IContainer
{
    public List<IItem> items { get; }

    public void Initialize();
}
