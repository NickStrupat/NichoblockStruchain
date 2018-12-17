using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NodeWebApi.DataModel
{
	public class Block : Block<String> {}

	public class Block<TData> : IEquatable<Block<TData>> where TData : IEquatable<TData>
	{
		public Int64 Id { get; private set; }
		public DateTime TimeStamp { get; private set; }
		public Byte[] PreviousHash { get; private set; }
		public TData Data { get; private set; }
		public Byte[] Hash { get; private set; }

		private Block() {}
		public Block(Byte[] previousHash, TData data)
		{
			TimeStamp = DateTime.UtcNow;
			PreviousHash = previousHash;
			Data = IsDataNullable && data == null ? throw new ArgumentNullException(nameof(data)) : data;
			Hash = CalculateHash();
		}

		public Boolean Equals(Block<TData> other) => !ReferenceEquals(null, other) && EqualsInternal(other);

		public override Boolean Equals(Object obj) => obj is Block<TData> other && EqualsInternal(other);

		private Boolean EqualsInternal(Block<TData> other)
		{
			if (ReferenceEquals(this, other))
				return true;
			return
				Id == other.Id &&
				TimeStamp.Equals(other.TimeStamp) &&
				Equals(PreviousHash, other.PreviousHash) &&
				EqualityComparer<TData>.Default.Equals(Data, other.Data) &&
				Equals(Hash, other.Hash);
		}

		private static readonly Boolean IsDataNullable = !typeof(TData).IsValueType;
		private TData SafeData => IsDataNullable ? Data ?? 

		public override Int32 GetHashCode() =>
			HashCode.Combine(
				Id,
				TimeStamp,
				PreviousHash ?? Array.Empty<Byte>(),
				Data,
				Hash ?? Array.Empty<Byte>()
			);
	}

	public class Context : DbContext
	{
		public DbSet<Block> Blocks { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite("Filename=UnicornClicker.db");
			//optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;");
		}
	}
}
