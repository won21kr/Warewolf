using System.ComponentModel;
using System.Linq;
using Dev2.Common.Interfaces.DB;
using Dev2.Common.Interfaces.Toolbox;
using Dev2.DataList.Contract;
using Dev2.Interfaces;
using Dev2.Services.Execution;
using Unlimited.Applications.BusinessDesignStudio.Activities;
using Warewolf.Core;
using Warewolf.Resource.Errors;

// ReSharper disable ConvertToAutoProperty

namespace Dev2.Activities
{
    [ToolDescriptorInfo("MicrosoftSQL", "SQL Server", ToolType.Native, "8999E59B-38A3-43BB-A98F-6090C5C9EA1E", "Dev2.Acitivities", "1.0.0.0", "Legacy", "Database", "/Warewolf.Studio.Themes.Luna;component/Images.xaml", "Tool_Database_SQL Server_Tags")]
    public class DsfSqlServerDatabaseActivity : DsfActivity
    {

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
        public IServiceExecution ServiceExecution { get; protected set; }
        public string ProcedureName { get; set; }

        public DsfSqlServerDatabaseActivity()
        {
            Type = "SQL Server Database";
            DisplayName = "SQL Server Database";
        }
        
        protected override void ExecutionImpl(IEsbChannel esbChannel, IDSFDataObject dataObject, string inputs, string outputs, out ErrorResultTO errors, int update)
        {
            var execErrors = new ErrorResultTO();

            errors = new ErrorResultTO();
            errors.MergeErrors(execErrors);
            if (string.IsNullOrEmpty(ProcedureName))
            {
                errors.AddError(ErrorResource.NoActionsInSelectedDB);
                return;
            }
            var databaseServiceExecution = ServiceExecution as DatabaseServiceExecution;
            if(databaseServiceExecution != null)
            {
                databaseServiceExecution.Inputs = Inputs.Select(a => new ServiceInput { EmptyIsNull = a.EmptyIsNull, Name = a.Name, RequiredField = a.RequiredField, Value = a.Value, TypeName = a.TypeName } as IServiceInput).ToList();
                databaseServiceExecution.Outputs = Outputs;
            }
            ServiceExecution.Execute(out execErrors, update);
            var fetchErrors = execErrors.FetchErrors();
            foreach(var error in fetchErrors)
            {
                dataObject.Environment.Errors.Add(error);
            }
            errors.MergeErrors(execErrors);
        }

        protected override void BeforeExecutionStart(IDSFDataObject dataObject, ErrorResultTO tmpErrors)
        {            
            base.BeforeExecutionStart(dataObject, tmpErrors);
            ServiceExecution = new DatabaseServiceExecution(dataObject);
            var databaseServiceExecution = ServiceExecution as DatabaseServiceExecution;
            databaseServiceExecution.ProcedureName = ProcedureName;
            ServiceExecution.GetSource(SourceId);
            ServiceExecution.BeforeExecution(tmpErrors);
        }

        protected override void AfterExecutionCompleted(ErrorResultTO tmpErrors)
        {
            base.AfterExecutionCompleted(tmpErrors);
            ServiceExecution.AfterExecution(tmpErrors);
        }


        public override enFindMissingType GetFindMissingType()
        {
            return enFindMissingType.DataGridActivity;
        }

    }
}