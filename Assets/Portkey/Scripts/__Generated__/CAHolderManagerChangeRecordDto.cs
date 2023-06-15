namespace Portkey.GraphQL
{
	public class CAHolderManagerChangeRecordDto
	{
		string caAddress {get; set;}
		string caHash {get; set;}
		string manager {get; set;}
		string changeType {get; set;}
		string blockHash {get; set;}
	}
}