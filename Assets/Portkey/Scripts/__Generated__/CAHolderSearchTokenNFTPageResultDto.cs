using System.Collections.Generic;
namespace Portkey.GraphQL
{
	public class CAHolderSearchTokenNFTPageResultDto
	{
		public long totalRecordCount {get; set;}
		public IList<CAHolderSearchTokenNFTDto> data {get; set;}
	}
}