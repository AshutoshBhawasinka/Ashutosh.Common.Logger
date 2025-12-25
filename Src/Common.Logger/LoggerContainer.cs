using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace Ashutosh.Common.Logger
{
    class LoggerContainer
    {
        internal static Container _container;
        internal class Container
        {
            [Import(typeof(ILogger))]
            public ILogger ActualLogger { get; set; }
        }

        internal static ILogger GetLogger()
        {
            if (_container?.ActualLogger == null)
            {
                Initialize();
            }
            return _container?.ActualLogger;
        }

        public static void Initialize()
        {
            _container = new Container();
            //An aggregate catalog that combines multiple catalogs  
            var catalog = new AggregateCatalog();

            //var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            //foreach (Assembly assembly in loadedAssemblies)
            //{
            //    if(assembly == Assembly.GetExecutingAssembly()) continue;
            //    Console.WriteLine("Adding assembly: " + assembly.FullName);
            //    catalog.Catalogs.Add(new AssemblyCatalog(assembly));
            //}
            
            catalog.Catalogs.Add(new ApplicationCatalog());

            var compositionContainer = new CompositionContainer(catalog);

            try
            {
                compositionContainer.ComposeParts(_container);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
            catch (TypeInitializationException te)
            {
                Console.WriteLine("TypeInitializationException: " + te.Message + Environment.NewLine + "Stack Trace: " + te.StackTrace);
                if (te.InnerException != null)
                {
                    var ie = te.InnerException;
                    Console.WriteLine("Inner exception: " + ie.Message + Environment.NewLine + "Stack Trace: " + ie.StackTrace);

                    if (te.InnerException.InnerException != null)
                    {
                        var ae = te.InnerException.InnerException;
                        Console.WriteLine("Inner exception: " + ae.Message + Environment.NewLine + "Stack Trace: " + ae.StackTrace);
                    }
                }
            }
            catch (ReflectionTypeLoadException te)
            {
                Console.WriteLine("TypeInitializationException: " + te.Message + Environment.NewLine + "Stack Trace: " + te.StackTrace);
                if (te.LoaderExceptions != null)
                {
                    foreach (Exception loaderException in te.LoaderExceptions)
                    {


                        var ie = loaderException;
                        Console.WriteLine("Inner Loader exception: " + ie.Message + Environment.NewLine + "Stack Trace: " +
                                          ie.StackTrace);

                        if (te.InnerException?.InnerException != null)
                        {
                            var ae = te.InnerException.InnerException;
                            Console.WriteLine("Inner Loader exception: " + ae.Message + Environment.NewLine + "Stack Trace: " +
                                              ae.StackTrace);
                        }
                    }
                }
            }
        }
    }
}
