namespace Portkey.GraphQL
{
	public class GuardianDto
	{
		int type {get; set;}
		string verifierId {get; set;}
		string identifierHash {get; set;}
		string salt {get; set;}
		bool isLoginGuardian {get; set;}
	}
}