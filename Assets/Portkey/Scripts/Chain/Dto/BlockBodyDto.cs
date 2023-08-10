using System.Collections.Generic;

namespace Portkey.Chain.Dto
{
    
    public class BlockBodyDto
    {
        public int TransactionsCount { get; set; }
            
        public List<string> Transactions { get; set; }
    }
}
