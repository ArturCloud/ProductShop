using Braintree;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX_Utility.BrainTree
{
    public class BrainTreeGate : IBrainTreeGate
    {
        public BrainTreeSettings options { get; set; }
        public IBraintreeGateway braintreeGateway { get; set; }  

        public BrainTreeGate(IOptions<BrainTreeSettings> options)
        {
            this.options = options.Value;
        }
        public IBraintreeGateway CreateGateway()
        {
            return new BraintreeGateway(options.Environment, options.MerchantId, options.PublicKey, options.PrivateKey);
        }

        public IBraintreeGateway GetGateway()
        {
            if(braintreeGateway == null)
            {
                braintreeGateway = CreateGateway();
            }
            return braintreeGateway;
        }
    }
}
