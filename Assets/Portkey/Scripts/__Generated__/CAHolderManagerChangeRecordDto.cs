namespace Portkey.GraphQL
{
	public class CAHolderManagerChangeRecordDto
	{
		public string caAddress {get; set;}
		public string caHash {get; set;}
		public string manager {get; set;}
		public string changeType {get; set;}
		public long blockHeight {get; set;}
		public string blockHash {get; set;}
	}
}