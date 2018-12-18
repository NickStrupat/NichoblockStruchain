using System;
using System.ComponentModel.DataAnnotations;

namespace NodeWebApi.DataModel
{
	public class Block : Block<String>
	{
		[Required]
		public override String Data
		{
			get => base.Data;
			protected set => base.Data = value;
		}

		public Block(String data) : base(data) {}
	}
}