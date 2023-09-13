using System.Collections.Generic;

namespace Azalea.Graphics;

public interface ICompositeDrawNode
{
	List<DrawNode>? Children { get; set; }
}
