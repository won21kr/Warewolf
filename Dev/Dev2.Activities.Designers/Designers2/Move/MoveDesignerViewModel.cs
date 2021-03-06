/*
*  Warewolf - Once bitten, there's no going back
*  Copyright 2016 by Warewolf Ltd <alpha@warewolf.io>
*  Licensed under GNU Affero General Public License 3.0 or later. 
*  Some rights reserved.
*  Visit our website for more information <http://warewolf.io/>
*  AUTHORS <http://warewolf.io/authors.php> , CONTRIBUTORS <http://warewolf.io/contributors.php>
*  @license GNU Affero General Public License <http://www.gnu.org/licenses/agpl-3.0.html>
*/

using System.Activities.Presentation.Model;
using Dev2.Activities.Designers2.Core;
using Dev2.Interfaces;

namespace Dev2.Activities.Designers2.Move
{
    public class MoveDesignerViewModel : FileActivityDesignerViewModel
    {
        public MoveDesignerViewModel(ModelItem modelItem)
            : base(modelItem, "File or Folder", "Destination")
        {
            AddTitleBarLargeToggle();
        }

        public override void Validate()
        {
            Errors = null;
            ValidateUserNameAndPassword();
            ValidateDestinationUsernameAndPassword();
            ValidateInputAndOutputPaths();
        }

        public override void UpdateHelpDescriptor(string helpText)
        {
            var mainViewModel = CustomContainer.Get<IMainViewModel>();
            if (mainViewModel != null)
            {
                mainViewModel.HelpViewModel.UpdateHelpText(helpText);
            }
        }
    }
}
