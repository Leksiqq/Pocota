using System.Collections;
using System.Reflection;

namespace Net.Leksi.Pocota.Client
{
    internal class ConnectorsMethodsList: IEnumerable<MethodInfo>
    {
        private readonly List<MethodInfo> _methods = [];
        public ConnectorsMethodsList()
        {
            Queue<Assembly> queue = new();
            HashSet<string> visited = [];
            queue.Enqueue(Assembly.GetEntryAssembly()!);
            while(queue.Count > 0)
            {
                Assembly assembly = queue.Dequeue();
                visited.Add(assembly.FullName!);
                foreach(Type type in assembly.GetTypes())
                {
                    if (typeof(Connector).IsAssignableFrom(type) && type != typeof(Connector))
                    {
                        foreach(MethodInfo method in type.GetMethods().Where(m => m.DeclaringType == type)) 
                        {
                            _methods.Add(method);
                        }
                    }
                }
                foreach(
                    AssemblyName? reference 
                    in assembly.GetReferencedAssemblies().Where(r => !visited.Contains(r.FullName))
                )
                {
                    try
                    {
                        queue.Enqueue(Assembly.Load(reference));
                    }
                    catch { }
                }
            }
        }

        public IEnumerator<MethodInfo> GetEnumerator()
        {
            return _methods.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _methods.GetEnumerator();
        }

    }
}