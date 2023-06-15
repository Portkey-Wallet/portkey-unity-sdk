namespace Portkey.GraphQL
{
	public class Guardian
	{
		string verifierId {get; set;}
		string identifierHash {get; set;}
		string salt {get; set;}
	}
}