using System.Data;
using Northwind.Common.DataModel;
using Northwind.Perf;

namespace ServiceStack.OrmLite.TestsPerf.Scenarios.Northwind
{
	public class SelectNorthwindSupplierScenario
		: DatabaseScenarioBase
	{
		private const int SupplierId = 1;

		protected override void Run(IDbCommand dbCmd)
		{
			if (this.IsFirstRun)
			{
				dbCmd.CreateTable<Supplier>(true);

				dbCmd.Insert(NorthwindFactory.Supplier(SupplierId, "Exotic Liquids", "Charlotte Cooper", "Purchasing Manager", "49 Gilbert St.", "London", null, "EC1 4SD", "UK", "(171) 555-2222", null, null));
			}

			dbCmd.GetById<Supplier>(SupplierId);
		}
	}
}