using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Dev2.Common;
using Dev2.Common.Interfaces;
using Dev2.Common.Interfaces.Core;
using Dev2.Common.Interfaces.Core.DynamicServices;
using Dev2.Common.Interfaces.DB;
using Dev2.Communication;
using Dev2.DynamicServices;
using Dev2.DynamicServices.Objects;
using Dev2.Runtime.Hosting;
using Dev2.Runtime.ServiceModel;
using Dev2.Runtime.ServiceModel.Data;
using Dev2.Workspaces;
using Warewolf.Core;

namespace Dev2.Runtime.ESB.Management.Services
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class FetchComPluginActions : IEsbManagementEndpoint
    {
        public string HandlesType()
        {
            return "FetchComPluginActions";
        }

        public StringBuilder Execute(Dictionary<string, StringBuilder> values, IWorkspace theWorkspace)
        {
            var serializer = new Dev2JsonSerializer();
            try
            {
                var pluginSource = serializer.Deserialize<ComPluginSourceDefinition>(values["source"]);
                var ns = serializer.Deserialize<INamespaceItem>(values["namespace"]);
                // ReSharper disable MaximumChainedReferences
                ComPluginServices services = new ComPluginServices();
                var src = ResourceCatalog.Instance.GetResource<ComPluginSource>(GlobalConstants.ServerWorkspaceID, pluginSource.Id);
                //src.AssemblyName = ns.FullName;
                ComPluginService svc = new ComPluginService();
                if (ns != null)
                {
                    svc.Namespace = ns.FullName; svc.Source = src;
                }
                else
                {
                    svc.Source = src;
                }
                var serviceMethodList = services.Methods(svc, Guid.Empty, Guid.Empty);
                var methods = serviceMethodList.Select(a => new PluginAction
                {
                    FullName = ns?.FullName,
                    Inputs = a.Parameters.Select(x =>
                    new ServiceInput(x.Name, x.DefaultValue ?? "")
                    {
                        Name = BuildServiceInputName(x.Name, x.TypeName)
                        ,
                        EmptyIsNull = x.EmptyToNull
                        ,
                        RequiredField = x.IsRequired
                        ,
                        TypeName = x.TypeName
                    } as IServiceInput).ToList(),
                    Method = a.Name,
                    Variables = a.Parameters.Select(x => new NameValue() { Name = x.Name + " (" + x.TypeName + ")", Value = "" } as INameValue).ToList(),
                } as IPluginAction).ToList();
                return serializer.SerializeToBuilder(new ExecuteMessage()
                {
                    HasError = false,
                    Message = serializer.SerializeToBuilder(methods)
                });
            }
            catch (Exception e)
            {

                return serializer.SerializeToBuilder(new ExecuteMessage()
                {
                    HasError = true,
                    Message = new StringBuilder(e.Message)
                });
            }
        }

        private string BuildServiceInputName(string name, string typeName)
        {
            try
            {
                var cleanTypeName = Type.GetType(typeName);
                // ReSharper disable once PossibleNullReferenceException
                return $"{name} ({cleanTypeName.Name})";
            }
            catch (Exception)
            {
                try
                {
                    var cleanTypeName = typeName.Contains("&") ? typeName.Split('&').First() : typeName.Split(',').First();
                    var newName = $"{name} ({cleanTypeName})";
                    return newName;
                }
                catch (Exception)
                {
                    return name;
                }

            }

        }

        public DynamicService CreateServiceEntry()
        {
            var findServices = new DynamicService { Name = HandlesType(), DataListSpecification = new StringBuilder("<DataList><Dev2System.ManagmentServicePayload ColumnIODirection=\"Both\"></Dev2System.ManagmentServicePayload></DataList>") };

            var fetchItemsAction = new ServiceAction { Name = HandlesType(), ActionType = enActionType.InvokeManagementDynamicService, SourceMethod = HandlesType() };

            findServices.Actions.Add(fetchItemsAction);

            return findServices;
        }

        public ResourceCatalog Resources => ResourceCatalog.Instance;
    }
}