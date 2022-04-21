using System.Linq;
using System.Collections.Generic;

namespace SimuladorBD {
    internal class Record {
        public List<FieldValue> Values { get; private set; }
        public List<Field> RecordStruct { get; set; }
        public string FullPath { get; private set; }
        public Record(List<Field> recordStruct, List<FieldValue> values) {
            this.RecordStruct = recordStruct;
            this.Values = values;
        }
        private void SortValues() {
            Values.OrderBy(d => {
                int index = RecordStruct.IndexOf(d.FieldType);
                return index == -1 ? int.MaxValue : index;
            });
        }
        override public string ToString() {
            SortValues();
            return string.Join(string.Empty, Values);
        }
    }
}
