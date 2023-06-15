using System.Collections.Generic;
namespace Portkey.GraphQL
{
	public class CAHolderTransactionPageResultDto
	{
		public long totalRecordCount {get; set;}
		public IList<CAHolderTransactionDto> data {get; set;}
	}
}