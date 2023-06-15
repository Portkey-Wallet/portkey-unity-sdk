using System.Collections.Generic;
namespace Portkey.GraphQL
{
	public class CAHolderSearchTokenNFTPageResultDto
	{
		long totalRecordCount {get; set;}
		IList<CAHolderSearchTokenNFTDto> data {get; set;}
	}
}