﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;


namespace Warewolf.UITests.Deploy
{
    [CodedUITest]
    public class DeployViewOnlyService
    {
        private const string WorkflowName = "DeployViewOnly";
        private const string GroupName = "Public";
        private const string RemoteServerAddress = "store.warewolf.io";
        private const string RemoteServerSourceName = "WarewolfStore";

        [TestMethod]
        public void DeployViewOnlyServiceUITest()
        {
            Uimap.Click_New_Workflow_Ribbon_Button();
            Uimap.Save_With_Ribbon_Button_And_Dialog(WorkflowName);
            Uimap.Click_Close_Workflow_Tab_Button();
            Uimap.SetResourcePermissions(WorkflowName, GroupName, true);
            Uimap.Click_Deploy_Ribbon_Button();
            Uimap.Click_Deploy_Tab_Destination_Server_Combobox();
            Uimap.Click_Deploy_Tab_Destination_Server_New_Remote_Server_Item();
            Uimap.CreateRemoteServerSource(RemoteServerSourceName, RemoteServerAddress, true);
            Uimap.Click_Deploy_Tab_Destination_Server_Combobox();
            Uimap.Click_Deploy_Tab_WarewolfStore_Item();
            Uimap.Click_Deploy_Tab_Destination_Server_Connect_Button();
            Uimap.Enter_Text_Into_Deploy_Source_Filter();
            Uimap.Select_Deploy_First_Source_Item();
            Uimap.Click_Deploy_Tab_Deploy_Button();
            Uimap.Click_Deploy_Tab_Source_Server_Combobox();
            Uimap.Click_Deploy_Tab_WarewolfStore_Item();
            Uimap.Select_Deploy_First_Source_Item();
            Uimap.Click_Deploy_Tab_Deploy_Button();
        }

        #region Additional test attributes
        
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Uimap.SetGlobalPlaybackSettings();
            Uimap.WaitForStudioStart();
            Console.WriteLine("Test \"" + TestContext.TestName + "\" starting on " + Environment.MachineName);
        }
        
        [TestCleanup()]
        public void MyTestCleanup()
        {
            Playback.PlaybackError -= Uimap.OnError;
            Uimap.TryCloseWorkflowTab();
            Uimap.TryCloseSettingsTab();
        }

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        private TestContext testContextInstance;

        UIMap Uimap
        {
            get
            {
                if (_uiMap == null)
                {
                    _uiMap = new UIMap();
                }

                return _uiMap;
            }
        }

        private UIMap _uiMap;

        #endregion
    }
}
