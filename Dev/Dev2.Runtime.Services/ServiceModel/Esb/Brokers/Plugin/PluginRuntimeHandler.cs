/*
*  Warewolf - Once bitten, there's no going back
*  Copyright 2016 by Warewolf Ltd <alpha@warewolf.io>
*  Licensed under GNU Affero General Public License 3.0 or later. 
*  Some rights reserved.
*  Visit our website for more information <http://warewolf.io/>
*  AUTHORS <http://warewolf.io/authors.php> , CONTRIBUTORS <http://warewolf.io/contributors.php>
*  @license GNU Affero General Public License <http://www.gnu.org/licenses/agpl-3.0.html>
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Dev2.Common;
using Dev2.Common.Interfaces.Core.Graph;
using Dev2.Data.Util;
using Dev2.Runtime.ServiceModel.Data;
using Newtonsoft.Json;
using Unlimited.Framework.Converters.Graph;

namespace Dev2.Runtime.ServiceModel.Esb.Brokers.Plugin
{

    /// <summary>
    /// Handler that invokes a plugin in its own app domain
    /// </summary>
    public class PluginRuntimeHandler : MarshalByRefObject, IRuntime
    {
        private readonly IAssemblyLoader _assemblyLoader;

        // ReSharper disable once MemberCanBePrivate.Global
        public PluginRuntimeHandler(IAssemblyLoader assemblyLoader)
        {
            _assemblyLoader = assemblyLoader;
        }

        public PluginRuntimeHandler()
            : this(new AssemblyLoader())
        {

        }

        string _assemblyLocation = "";
        /// <summary>
        /// Runs the specified setup information.
        /// </summary>
        /// <param name="setupInfo">The setup information.</param>
        /// <returns></returns>
        public object Run(PluginInvokeArgs setupInfo)
        {
            Assembly loadedAssembly;
            _assemblyLocation = setupInfo.AssemblyLocation;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            if (!_assemblyLoader.TryLoadAssembly(setupInfo.AssemblyLocation, setupInfo.AssemblyName, out loadedAssembly))
            {
                return null;
            }
            object pluginResult;
            var methodToRun = ExecutePlugin(setupInfo, loadedAssembly, out pluginResult);
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            var formater = setupInfo.OutputFormatter;
            if (formater != null)
            {
                pluginResult = AdjustPluginResult(pluginResult, methodToRun);
                return formater.Format(pluginResult).ToString(); 
            }
            pluginResult = JsonConvert.SerializeObject(pluginResult);
            return pluginResult;
        }

        public IOutputDescription Test(PluginInvokeArgs setupInfo,out string jsonResult)
        {
            try
            {
                Assembly loadedAssembly;
                jsonResult = null;
                _assemblyLocation = setupInfo.AssemblyLocation;
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                if (!_assemblyLoader.TryLoadAssembly(setupInfo.AssemblyLocation, setupInfo.AssemblyName, out loadedAssembly))
                {
                    return null;
                }
                object pluginResult;
                var methodToRun = ExecutePlugin(setupInfo, loadedAssembly, out pluginResult);

                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
                // do formating here to avoid object serialization issues ;)
                var dataBrowser = DataBrowserFactory.CreateDataBrowser();
                var dataSourceShape = DataSourceShapeFactory.CreateDataSourceShape();

                if (pluginResult != null)
                {
                    jsonResult = JsonConvert.SerializeObject(pluginResult);
                    pluginResult = AdjustPluginResult(pluginResult, methodToRun);
                    var tmpData = dataBrowser.Map(pluginResult);
                    dataSourceShape.Paths.AddRange(tmpData);

                }
                
                var result = OutputDescriptionFactory.CreateOutputDescription(OutputFormats.ShapedXML);
                result.DataSourceShapes.Add(dataSourceShape);
                return result;
            }
            catch (Exception e)
            {
                Dev2Logger.Error("IOutputDescription Test(PluginInvokeArgs setupInfo)", e);
                jsonResult = null;
                return null;
            }
        }

        private MethodInfo ExecutePlugin(PluginInvokeArgs setupInfo, Assembly loadedAssembly, out object pluginResult)
        {
            var typeList = BuildTypeList(setupInfo.Parameters);
            var type = loadedAssembly.GetType(setupInfo.Fullname);
            var valuedTypeList = new List<object>();
            foreach(var methodParameter in setupInfo.Parameters)
            {
                try
                {
                    var anonymousType = JsonConvert.DeserializeObject(methodParameter.Value, Type.GetType(methodParameter.TypeName));
                    if(anonymousType != null)
                    {
                        valuedTypeList.Add(anonymousType);
                    }
                }
                catch(Exception)
                {
                    valuedTypeList.Add(methodParameter.Value);
                }
            }
            var methodToRun = type.GetMethod(setupInfo.Method, typeList.ToArray());
            if(methodToRun==null && typeList.Count == 0)
            {
                methodToRun = type.GetMethod(setupInfo.Method);
            }
            object instance = Activator.CreateInstance(type);

            if(methodToRun != null)
            {
                pluginResult = methodToRun.Invoke(instance, BindingFlags.InvokeMethod, null, valuedTypeList.ToArray(), CultureInfo.CurrentCulture);
                return methodToRun;
            }
            pluginResult = null;
            return null;
        }

        // ReSharper disable once InconsistentNaming
        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string[] tokens = args.Name.Split(",".ToCharArray());
            Debug.WriteLine("Resolving : " + args.Name);
            var directoryName = Path.GetDirectoryName(_assemblyLocation);
            return Assembly.LoadFile(Path.Combine(new[] { directoryName, tokens[0] + ".dll" }));
        }
        /// <summary>
        /// Lists the namespaces.
        /// </summary>
        /// <param name="assemblyLocation">The assembly location.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns></returns>
        public List<string> ListNamespaces(string assemblyLocation, string assemblyName)
        {
            try
            {
                Assembly loadedAssembly;
                List<string> namespaces = new List<string>();
                if (_assemblyLoader.TryLoadAssembly(assemblyLocation, assemblyName, out loadedAssembly))
                {
                    // ensure we flush out the rubbish that GAC brings ;)
                    namespaces = loadedAssembly.GetTypes()
                        .Select(t => t.FullName)
                        .Distinct()
                        .Where(q => q.IndexOf("`", StringComparison.Ordinal) < 0
                                    && q.IndexOf("+", StringComparison.Ordinal) < 0
                                    && q.IndexOf("<", StringComparison.Ordinal) < 0
                                    && !q.StartsWith("_")).ToList();
                }
                return namespaces;
            }
            catch (BadImageFormatException e)
            {
                Dev2Logger.Error(e);
                throw;
            }
        }

        /// <summary>
        /// Lists the methods.
        /// </summary>
        /// <param name="assemblyLocation">The assembly location.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="fullName">The full name.</param>
        /// <returns></returns>
        public ServiceMethodList ListMethods(string assemblyLocation, string assemblyName, string fullName)
        {
            Assembly assembly;
            var serviceMethodList = new ServiceMethodList();
            if (_assemblyLoader.TryLoadAssembly(assemblyLocation, assemblyName, out assembly))
            {
                var type = assembly.GetType(fullName);
                var methodInfos = type.GetMethods();

                methodInfos.ToList().ForEach(info =>
                {
                    var serviceMethod = new ServiceMethod { Name = info.Name };
                    var parameterInfos = info.GetParameters().ToList();
                    parameterInfos.ForEach(parameterInfo =>
                        serviceMethod.Parameters.Add(
                            new MethodParameter
                            {
                                DefaultValue = parameterInfo.DefaultValue == null ? string.Empty : parameterInfo.DefaultValue.ToString(),
                                EmptyToNull = false,
                                IsRequired = true,
                                Name = parameterInfo.Name,
                                TypeName = parameterInfo.ParameterType.AssemblyQualifiedName
                            }));
                    serviceMethodList.Add(serviceMethod);
                });
            }

            return serviceMethodList;
        }

        /// <summary>
        /// Fetches the name space list object.
        /// </summary>
        /// <param name="pluginSource">The plugin source.</param>
        /// <returns></returns>
        public NamespaceList FetchNamespaceListObject(PluginSource pluginSource)
        {
            var interrogatePlugin = ReadNamespaces(pluginSource.AssemblyLocation, pluginSource.AssemblyName);
            var namespacelist = new NamespaceList();
            namespacelist.AddRange(interrogatePlugin);
            return namespacelist;
        }


        /// <summary>
        /// Adjusts the plugin result.
        /// </summary>
        /// <param name="pluginResult">The plugin result.</param>
        /// <param name="methodToRun">The method automatic run.</param>
        /// <returns></returns>
        private object AdjustPluginResult(object pluginResult, MethodInfo methodToRun)
        {
            object result = pluginResult;
            // When it returns a primitive or string and it is not XML or JSON, make it so ;)
            if ((methodToRun.ReturnType.IsPrimitive || methodToRun.ReturnType.FullName == "System.String")
                && !DataListUtil.IsXml(pluginResult.ToString()) && !DataListUtil.IsJson(pluginResult.ToString()))
            {
                // add our special tags ;)
                result = string.Format("<{0}>{1}</{2}>", GlobalConstants.PrimitiveReturnValueTag, pluginResult, GlobalConstants.PrimitiveReturnValueTag);
            }

            return result;
        }

        /// <summary>
        /// Reads the namespaces.
        /// </summary>
        /// <param name="assemblyLocation">The assembly location.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns></returns>
        private IEnumerable<NamespaceItem> ReadNamespaces(string assemblyLocation, string assemblyName)
        {
            try
            {
                var result = new List<NamespaceItem>();
                var list = ListNamespaces(assemblyLocation, assemblyName);
                list.ForEach(fullName =>
                    result.Add(new NamespaceItem
                    {
                        AssemblyLocation = assemblyLocation,
                        AssemblyName = assemblyName,
                        FullName = fullName
                    }));

                return result;
            }
            // ReSharper disable once RedundantCatchClause
            catch (BadImageFormatException)
            {
                throw;
            }
        }

        private Type DeriveType(string typename)
        {
            var type = Type.GetType(typename, true);
            return type;
        }

        /// <summary>
        /// Builds the type list.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        private List<Type> BuildTypeList(IEnumerable<MethodParameter> parameters)
        {
            var typeList = new List<Type>();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var methodParameter in parameters)
            {
                var type = DeriveType(methodParameter.TypeName);
                typeList.Add(type);
            }
            return typeList;
        }

        /*
        /// <summary>
        /// Tries the load assembly.
        /// </summary>
        /// <param name="assemblyLocation">The assembly location.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="loadedAssembly">The loaded assembly.</param>
        /// <returns></returns>
        public bool TryLoadAssembly(string assemblyLocation, string assemblyName, out Assembly loadedAssembly)
        {
            loadedAssembly = null;

            if (assemblyLocation != null && assemblyLocation.StartsWith(GlobalConstants.GACPrefix))
            {
                try
                {
                    loadedAssembly = Assembly.Load(assemblyName);
                    LoadDepencencies(loadedAssembly, assemblyLocation);
                    return true;
                }
                catch (System.BadImageFormatException e)//WOLF-1640
                {
                    Dev2Logger.Error(e);
                    throw;

                }
                catch (Exception e)
                {
                    Dev2Logger.Error(e.Message);
                }
            }
            else
            {
                try
                {
                    if (assemblyLocation != null)
                    {
                        loadedAssembly = Assembly.LoadFrom(assemblyLocation);
                        LoadDepencencies(loadedAssembly, assemblyLocation);
                    }
                    return true;
                }
                catch (System.BadImageFormatException e)//WOLF-1640
                {
                    Dev2Logger.Error(e);
                    throw;
                }
                catch
                {
                    try
                    {
                        if (assemblyLocation != null)
                        {
                            loadedAssembly = Assembly.UnsafeLoadFrom(assemblyLocation);
                            LoadDepencencies(loadedAssembly, assemblyLocation);
                        }
                        return true;
                    }
                    catch (Exception e)
                    {
                        Dev2Logger.Error(e);
                    }
                }
                try
                {
                    if (assemblyLocation != null)
                    {
                        var objHAndle = Activator.CreateInstanceFrom(assemblyLocation, assemblyName);
                        var loadedObject = objHAndle.Unwrap();
                        loadedAssembly = Assembly.GetAssembly(loadedObject.GetType());
                    }
                    LoadDepencencies(loadedAssembly, assemblyLocation);
                    return true;
                }
                catch (System.BadImageFormatException e)//WOLF-1640
                {
                    Dev2Logger.Error(e);
                    throw;
                }
                catch (Exception e)
                {
                    Dev2Logger.Error(e);
                }
            }
            return false;
        }


        /// <summary>
        /// Loads the dependencies.
        /// </summary>
        /// <param name="asm">The asm.</param>
        /// <param name="assemblyLocation">The assembly location.</param>
        /// <exception cref="System.Exception">Could not locate Assembly [  + assemblyLocation +  ]</exception>
        private void LoadDepencencies(Assembly asm, string assemblyLocation)
        {
            // load dependencies ;)
            if (asm != null)
            {
                var toLoadAsm = asm.GetReferencedAssemblies();

                foreach (var toLoad in toLoadAsm)
                {
                    var fullName = toLoad.FullName;
                    if (_loadedAssemblies.Contains(fullName))
                    {
                        continue;
                    }
                    Assembly depAsm = null;
                    try
                    {
                        depAsm = Assembly.Load(toLoad);
                    }
                    catch
                    {
                        var path = Path.GetDirectoryName(assemblyLocation);
                        if (path != null)
                        {
                            var myLoad = Path.Combine(path, toLoad.Name + ".dll");
                            depAsm = Assembly.LoadFrom(myLoad);
                        }
                    }
                    if (depAsm != null)
                    {
                        if (!_loadedAssemblies.Contains(fullName))
                        {
                            _loadedAssemblies.Add(fullName);
                        }
                        LoadDepencencies(depAsm, assemblyLocation);
                    }
                }
            }
            else
            {
                throw new Exception("Could not locate Assembly [ " + assemblyLocation + " ]");
            }
        }*/
    }
}
