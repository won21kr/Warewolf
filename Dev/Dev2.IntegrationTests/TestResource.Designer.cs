﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Dev2.Integration.Tests {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class TestResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal TestResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Dev2.Integration.Tests.TestResource", typeof(TestResource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Service Name=&quot;DeleteWorkflowTest&quot;&gt;
        ///  &lt;Action Name=&quot;InvokeWorkflow&quot; Type=&quot;Workflow&quot;&gt;
        ///    &lt;XamlDefinition&gt;&amp;lt;Activity mc:Ignorable=&quot;sap&quot; x:Class=&quot;DeleteWorkflowTest&quot; xmlns=&quot;http://schemas.microsoft.com/netfx/2009/xaml/activities&quot; xmlns:av=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot; xmlns:dsca=&quot;clr-namespace:Dev2.Studio.Core.Activities;assembly=Dev2.Studio.Core.Activities&quot; xmlns:mc=&quot;http://schemas.openxmlformats.org/markup-compatibility/2006&quot; xmlns:mva=&quot;clr-namespace:Microsoft.VisualBasic.A [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DeleteWorkflowTest {
            get {
                return ResourceManager.GetString("DeleteWorkflowTest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to I73573r0.
        /// </summary>
        internal static string PathOperations_Correct_Password {
            get {
                return ResourceManager.GetString("PathOperations_Correct_Password", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to IntegrationTester.
        /// </summary>
        internal static string PathOperations_Correct_Username {
            get {
                return ResourceManager.GetString("PathOperations_Correct_Username", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to http://localhost:3142/services/.
        /// </summary>
        internal static string WebserverURI_Local {
            get {
                return ResourceManager.GetString("WebserverURI_Local", resourceCulture);
            }
        }
    }
}
