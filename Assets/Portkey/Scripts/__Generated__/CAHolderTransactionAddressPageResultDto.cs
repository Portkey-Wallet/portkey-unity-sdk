using System.Collections.Generic;
namespace Portkey.GraphQL
{
	public class CAHolderTransactionAddressPageResultDto
	{
		public long totalRecordCount {get; set;}
		public IList<CAHolderTransactionAddressDto> data {get; set;}
	}
}