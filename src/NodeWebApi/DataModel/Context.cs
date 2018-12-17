using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NodeWebApi.DataModel
{
	public class Node
	{
		public virtual Int64 Id { get; private set; }

		[Required]
		public virtual Byte[] PublicKey { get; protected set; }
		[Required]
		public virtual Byte[] PrivateKey { get; protected set; }

		public Node(Byte[] publicKey, Byte[] privateKey)
		{
			PublicKey = publicKey;
			PrivateKey = privateKey;
		}

		public virtual HashSet<Block> Blocks { get; set; } = new HashSet<Block>();
	}

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

	public class Block<TData> : IEquatable<Block<TData>> where TData : IEquatable<TData>
	{
		public virtual Int64 Id { get; private set; }
		public virtual Int64 NodeId { get; private set; }
		public virtual DateTime TimeStamp { get; protected set; } = DateTime.UtcNow;
		//public virtual Byte[] PreviousHash { get; protected set; }
		public virtual TData Data { get; protected set; }
		//public virtual Byte[] Hash { get; protected set; }

		public virtual Node Node { get; set; }

		private Block() {}

		public Block(TData data) =>
			Data = IsDataNullable && data == null ? throw new ArgumentNullException(nameof(data)) : data;

		//public Block(Byte[] previousHash, TData data)
		//{
		//	TimeStamp = DateTime.UtcNow;
		//	PreviousHash = previousHash;
		//	Data = IsDataNullable && data == null ? throw new ArgumentNullException(nameof(data)) : data;
		//	Hash = CalculateHash();
		//}

		public Boolean Equals(Block<TData> other) => !ReferenceEquals(null, other) && EqualsInternal(other);

		public override Boolean Equals(Object obj) => obj is Block<TData> other && EqualsInternal(other);

		private Boolean EqualsInternal(Block<TData> other)
		{
			if (ReferenceEquals(this, other))
				return true;
			return
				Id == other.Id &&
				TimeStamp.Equals(other.TimeStamp) &&
				//Equals(PreviousHash, other.PreviousHash) &&
				EqualityComparer<TData>.Default.Equals(Data, other.Data);// &&
				//Equals(Hash, other.Hash);
		}

		private static readonly Boolean IsDataNullable = !typeof(TData).IsValueType;
		//private TData SafeData => IsDataNullable ? Data ?? 

		public override Int32 GetHashCode() =>
			HashCode.Combine(
				Id,
				TimeStamp,
				//PreviousHash ?? Array.Empty<Byte>(),
				Data//,
				//Hash ?? Array.Empty<Byte>()
			);
	}

	public class Context : DbContext
	{
		public virtual DbSet<Node> Nodes { get; set; }
		public virtual DbSet<Block> Blocks { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite($"Filename={GetType().FullName}.db");
			//optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Block>()
				.HasIndex(b => b.TimeStamp);

			modelBuilder.Entity<Block>()
				.HasOne(b => b.Node)
				.WithMany(n => n.Blocks)
				.HasForeignKey(b => b.NodeId)
				.HasPrincipalKey(n => n.Id);

			modelBuilder.Entity<Block>()
				.HasAlternateKey(b => new {b.Id, b.NodeId});
		}
	}
}
