using System.Collections.Generic;
namespace Portkey.GraphQL
{
	public class CAHolderTransactionAddressPageResultDto
	{
		long totalRecordCount {get; set;}
		IList<CAHolderTransactionAddressDto> data {get; set;}
	}
}