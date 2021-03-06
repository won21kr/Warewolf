﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by coded UI test builder.
//      Version: 11.0.0.0
//
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------

using System;
using System.CodeDom.Compiler;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
using Mouse = Microsoft.VisualStudio.TestTools.UITesting.Mouse;

namespace Dev2.Studio.UI.Tests.UIMaps
{
    [GeneratedCode("Coded UITest Builder", "11.0.50727.1")]
    public partial class WorkflowDesignerUIMap
    {

        /// <summary>
        /// Gets the adorner hover point.
        /// </summary>
        /// <param name="boundingRectangle">The bounding rectangle.</param>
        /// <returns></returns>
        public void MoveMouseForAdornersToAppear(Rectangle boundingRectangle)
        {
            var p =  new Point((boundingRectangle.X + 25), (boundingRectangle.Y + 60));

            Mouse.Move(p);
        }

        private WpfButton GetQuickVariableInputButton(UITestControl theTab, string controlAutomationId)
        {
            UITestControl aControl = FindControlByAutomationId(theTab, controlAutomationId);
            WpfButton controlButtons = new WpfButton(aControl);
            UITestControlCollection buttonCollection = controlButtons.FindMatchingControls();
            foreach(WpfButton theButton in buttonCollection)
            {
                if (theButton.AutomationId.Contains("QuickVariableAddBtn_AutoID"))
                {
                    return theButton;
                }
            }
            throw new Exception("Error - The Quick Variable Input button could not be located!");
        }
    }
}
