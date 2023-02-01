using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX_Data.Initializer
{
    public interface IDbInitializer 
    {
        void Initialize();
    }
}
