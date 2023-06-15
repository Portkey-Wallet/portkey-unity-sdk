using System.Collections.Generic;
namespace Portkey.GraphQL
{
	public class CAHolderNFTBalancePageResultDto
	{
		public long totalRecordCount {get; set;}
		public IList<CAHolderNFTBalanceInfoDto> data {get; set;}
	}
}