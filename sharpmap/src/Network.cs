using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap
{
	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class Init
	{

	}

	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class LayerData
	{

	}

	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class LayerUpdate
	{
		public int LayerId { get; set; }
		public int RegionId { get; set; }
	}
}