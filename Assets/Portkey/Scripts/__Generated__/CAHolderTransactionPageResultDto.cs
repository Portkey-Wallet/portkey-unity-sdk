using System.Collections.Generic;
namespace Portkey.GraphQL
{
	public class CAHolderTransactionPageResultDto
	{
		long totalRecordCount {get; set;}
		IList<CAHolderTransactionDto> data {get; set;}
	}
}