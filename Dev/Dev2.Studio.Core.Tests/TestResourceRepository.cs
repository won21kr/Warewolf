/*
*  Warewolf - Once bitten, there's no going back
*  Copyright 2016 by Warewolf Ltd <alpha@warewolf.io>
*  Licensed under GNU Affero General Public License 3.0 or later. 
*  Some rights reserved.
*  Visit our website for more information <http://warewolf.io/>
*  AUTHORS <http://warewolf.io/authors.php> , CONTRIBUTORS <http://warewolf.io/contributors.php>
*  @license GNU Affero General Public License <http://www.gnu.org/licenses/agpl-3.0.html>
*/

using Dev2.Studio.Core.Interfaces;
using Moq;

// ReSharper disable once CheckNamespace
namespace Dev2.Studio.Core.AppResources.Repositories
{
    class TestResourceRepository : ResourceRepository
    {
        public TestResourceRepository()
            : this(new Mock<IEnvironmentModel>().Object)
        {
        }

        public TestResourceRepository(IEnvironmentModel environmentModel)
            : base(environmentModel)
        {
        }

        public int LoadResourcesHitCount { get; private set; }

        protected override void LoadResources()
        {
            LoadResourcesHitCount++;
            base.LoadResources();
        }
    }
}
