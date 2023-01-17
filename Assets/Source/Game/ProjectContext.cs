using System;
using System.Collections.Generic;

namespace Source
{
    public class ProjectContext
    {
        private static ProjectContext _instance;
        public static ProjectContext Container => _instance ??= new ProjectContext();
        
        private static readonly Dictionary<Type, object> Implementations = new();

        public void RegisterAsSingle<T>(T implementation) where T : notnull
        {
            if(HasTypeImplementationSingle<T>())
                throw new InvalidOperationException("There is implementation for this type.");

            Implementations.Add(typeof(T), implementation);
        }
        
        public void UnRegisterSingle<T>() where T : notnull
        {
            if (!HasTypeImplementationSingle<T>())
                throw new InvalidOperationException("There is no implementation for this type.");
            
            Implementations.Remove(typeof(T));
        }
        
        public T GetSingle<T>() where T : notnull => 
            HasTypeImplementationSingle<T>() ? (T)Implementations[typeof(T)] : default;

        public static bool HasTypeImplementationSingle<T>() => 
            Implementations.ContainsKey(typeof(T));
    }
}