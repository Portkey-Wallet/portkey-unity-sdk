namespace Portkey.GraphQL
{
	public class Guardian
	{
		public int type {get; set;}
		public string verifierId {get; set;}
		public string identifierHash {get; set;}
		public string salt {get; set;}
		public bool isLoginGuardian {get; set;}
	}
}