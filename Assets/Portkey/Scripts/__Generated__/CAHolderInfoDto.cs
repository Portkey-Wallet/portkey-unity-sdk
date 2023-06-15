using System.Collections.Generic;
namespace Portkey.GraphQL
{
	public class CAHolderInfoDto
	{
		public string id {get; set;}
		public string chainId {get; set;}
		public string caHash {get; set;}
		public string caAddress {get; set;}
		public IList<ManagerInfo> managerInfos {get; set;}
		public string originChainId {get; set;}
		public GuardianList guardianList {get; set;}
	}
}