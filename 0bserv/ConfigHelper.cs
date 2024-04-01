namespace _0bserv
{
	public class _0bservConfigHelper
	{
		private IConfiguration _configuration;

		public _0bservConfigHelper(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public static string GetConnectionString()
		{
			// retrieve nested App Service app setting
			var myHierarchicalConfig = _configuration["ConnectionStrings"];
			// retrieve App Service connection string
			var myConnString = _configuration.GetConnectionString("MSSQL");
			return myConnString;
		}
	}
}