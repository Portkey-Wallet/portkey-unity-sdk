using System.Collections.Generic;
namespace Portkey.GraphQL
{
	public class CAHolderInfoDto
	{
		string id {get; set;}
		string chainId {get; set;}
		string caHash {get; set;}
		string caAddress {get; set;}
		IList<ManagerInfo> managerInfos {get; set;}
		string originChainId {get; set;}
		GuardianList guardianList {get; set;}
	}
}