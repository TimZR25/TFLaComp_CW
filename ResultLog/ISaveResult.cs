using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFLaComp_1.DTO;

namespace TFLaComp_1.ResultLog
{
    public interface ISaveResult
    {
       void WriteToLog(List<FullCardDTO> cards);
    }
}
