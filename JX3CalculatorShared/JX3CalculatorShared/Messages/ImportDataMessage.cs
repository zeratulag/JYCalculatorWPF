using JX3CalculatorShared.Class;
using JX3CalculatorShared.ViewModels;

namespace JX3CalculatorShared.Messages
{

    public class ImportDataMessage
    {
        public ImportDataType ImportType { get; }
        public object Data { get; }

        public ImportDataMessage(ImportDataType importType, object data)
        {
            ImportType = importType;
            Data = data;
        }

        public ImportDataMessage(ImportDataType importType, JBBB data) : this(importType, (object) data)
        {
        }
    }
}