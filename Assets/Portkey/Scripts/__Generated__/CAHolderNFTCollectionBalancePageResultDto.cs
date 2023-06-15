using System.Collections.Generic;
namespace Portkey.GraphQL
{
	public class CAHolderNFTCollectionBalancePageResultDto
	{
		public long totalRecordCount {get; set;}
		public IList<CAHolderNFTCollectionBalanceInfoDto> data {get; set;}
	}
}