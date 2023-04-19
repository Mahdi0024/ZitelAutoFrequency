using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZitelUtilLib.Responses;

internal class QueryFrequencyResponse : ZitelResponse
{
    public string LockedStatus { get; set; } = "";
    public string FreqPoint { get; set; } = "";
    public string PhyCellId { get; set; } = "";
}
