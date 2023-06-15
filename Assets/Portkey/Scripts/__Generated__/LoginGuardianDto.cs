namespace Portkey.GraphQL
{
	public class LoginGuardianDto
	{
		string id {get; set;}
		string chainId {get; set;}
		string caHash {get; set;}
		string caAddress {get; set;}
		string manager {get; set;}
		GuardianDto loginGuardian {get; set;}
	}
}