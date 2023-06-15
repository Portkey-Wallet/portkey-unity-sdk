using System.Collections.Generic;
namespace Portkey.GraphQL
{
	public class CAHolderTokenBalancePageResultDto
	{
		public long totalRecordCount {get; set;}
		public IList<CAHolderTokenBalanceDto> data {get; set;}
	}
}