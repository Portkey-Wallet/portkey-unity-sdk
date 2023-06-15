using System.Collections.Generic;
namespace Portkey.GraphQL
{
	public class CAHolderTokenBalancePageResultDto
	{
		long totalRecordCount {get; set;}
		IList<CAHolderTokenBalanceDto> data {get; set;}
	}
}