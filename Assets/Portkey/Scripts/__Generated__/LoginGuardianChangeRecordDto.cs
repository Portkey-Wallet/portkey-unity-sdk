namespace Portkey.GraphQL
{
	public class LoginGuardianChangeRecordDto
	{
		string changeType {get; set;}
		long blockHeight {get; set;}
		string blockHash {get; set;}
		string id {get; set;}
		string chainId {get; set;}
		string caHash {get; set;}
		string caAddress {get; set;}
		string manager {get; set;}
		GuardianDto loginGuardian {get; set;}
	}
}