
using System.Collections.Generic;

namespace Registries
{
	internal class Registry<T> where T : IRegisterable
	{
		private readonly Dictionary<int, T> instances = new();
		private static int nextID = 1;

		internal int Register(T instance)
		{
			int id = nextID++;
			instances.Add(id, instance);
			return id;
		}

		internal T GetById(int id)
		{
			instances.TryGetValue(id, out T instance);
			return instance;
		}
	}
}
