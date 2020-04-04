using System.Collections.Generic;
using ExcelProcessingModule.Models;
using Prism.Events;

namespace ExcelProcessingModule.Events
{
    public class DataTableSubEvent: PubSubEvent<List<ExcelData>>
    {
    }
}
