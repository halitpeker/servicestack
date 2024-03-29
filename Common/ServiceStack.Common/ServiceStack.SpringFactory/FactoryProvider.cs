using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Configuration;
using ServiceStack.Logging;

namespace ServiceStack.SpringFactory
{
	public class FactoryProvider 
		: IFactoryProvider
	{
		private readonly ILog log = LogManager.GetLogger(typeof(FactoryProvider));

		private IObjectFactory Factory { get; set; }
		private readonly object readWriteLock = new object();
		List<IDisposable> Disposables { get; set; }
		private Dictionary<string, object> ConfigInstanceCache { get; set; }
		private Dictionary<Type, object> RuntimeInstanceCache { get; set; }
		private Dictionary<Type, Type> TypeMapLookup { get; set; }

		public bool AutoDispose { get; set; }

		public FactoryProvider(FactoryProvider cloneProvider, params object[] providers)
			: this(new EmptyObjectFactory())
		{
			RegisterAll(cloneProvider.RuntimeInstanceCache.Values.ToArray());
			RegisterAll(providers);
		}

		public FactoryProvider(params object[] providers)
			: this(new EmptyObjectFactory(), providers)
		{
		}

		public FactoryProvider(IObjectFactory factory, params object[] providers)
			: this(factory)
		{
			RegisterAll(providers);
		}

		private void RegisterAll(object[] providers)
		{
			foreach (var provider in providers)
			{
				Register(provider);
			}
		}

		public FactoryProvider(IObjectFactory factory)
		{
			this.Factory = factory;
			this.ConfigInstanceCache = new Dictionary<string, object>();
			this.RuntimeInstanceCache = new Dictionary<Type, object>();
			this.TypeMapLookup = new Dictionary<Type, Type>();
			this.Disposables = new List<IDisposable>();
		}

		/// <summary>
		/// Registers the specified provider.
		/// </summary>
		/// <param name="provider">The provider.</param>
		public void Register<T>(T provider)
		{
			var key = typeof(T) == typeof(object) ? provider.GetType() : typeof(T);
			lock (readWriteLock)
			{
				this.RuntimeInstanceCache[key] = provider;
				RegisterDisposable(provider);
			}
		}

		private void RegisterDisposable<T>(T provider)
		{
			var disposable = provider as IDisposable;
			if (disposable != null)
			{
				this.Disposables.Add(disposable);
			}
		}

		private Type FindTypeLookup(Type resolveType)
		{
			if (!this.TypeMapLookup.ContainsKey(resolveType))
			{
				foreach (var cacheEntry in this.RuntimeInstanceCache)
				{
					if (!resolveType.IsAssignableFrom(cacheEntry.Value.GetType())) continue;
					this.TypeMapLookup[resolveType] = cacheEntry.Key;
					return cacheEntry.Key;
				}
				return null;
			}
			return this.TypeMapLookup[resolveType];
		}

		/// <summary>
		/// Resolves this instance based on the typeof(T)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T Resolve<T>()
		{
			var key = FindTypeLookup(typeof(T));
			if (key != null && this.RuntimeInstanceCache.ContainsKey(key))
			{
				return (T)this.RuntimeInstanceCache[key];
			}
			//This will throw an exception if there is more than 1 match for typeof(T)
			return this.Factory != null ? this.Factory.Create<T>() : default(T);
		}

		public object Resolve(Type type)
		{
			var key = FindTypeLookup(type);
			if (key != null && this.RuntimeInstanceCache.ContainsKey(key))
			{
				return this.RuntimeInstanceCache[key];
			}
			//This will throw an exception if there is more than 1 match for typeof(T)
			return this.Factory != null ? this.Factory.Create(type) : null;
		}

		/// <summary>
		/// Gets a cached instance of a factory object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public T Resolve<T>(string name)
		{
			AssertFactory();
			if (!ConfigInstanceCache.ContainsKey(name))
			{
				ConfigInstanceCache[name] = Create<T>(name);
			}
			return (T)ConfigInstanceCache[name];
		}

		/// <summary>
		/// Resolves the optional.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name">The name.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns></returns>
		public T ResolveOptional<T>(string name, T defaultValue)
		{
			AssertFactory();
			return Factory.Contains(name) ? Resolve<T>(name) : defaultValue;
		}

		/// <summary>
		/// Creates a new instance of a factory object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public T Create<T>(string name)
		{
			AssertFactory();
			lock (readWriteLock)
			{
				var instance = Factory.Create<T>(name);
				RegisterDisposable(instance);
				return instance;
			}
		}

		private void AssertFactory()
		{
			if (this.Factory == null)
			{
				throw new NotSupportedException("this requires 'Factory' needs to be initialized");
			}
		}

		~FactoryProvider()
		{
			if (this.AutoDispose)
			{
				Dispose(false);
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			foreach (var disposable in this.Disposables)
			{
				try
				{
					disposable.Dispose();

				}
				catch (Exception ex)
				{
					log.Error(string.Format("Error disposing of type '{0}'", disposable.GetType().Name), ex);
					throw;
				}
			}
			this.Disposables.Clear();
		}
	}
}