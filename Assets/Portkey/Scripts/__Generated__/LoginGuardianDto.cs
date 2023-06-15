namespace Portkey.GraphQL
{
	public class LoginGuardianDto
	{
		public string id {get; set;}
		public string chainId {get; set;}
		public string caHash {get; set;}
		public string caAddress {get; set;}
		public string manager {get; set;}
		public GuardianDto loginGuardian {get; set;}
	}
}